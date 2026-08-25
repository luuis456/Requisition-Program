using WarehouseRequisition.Common;
using WarehouseRequisition.Enums;
using WarehouseRequisition.Models;
using WarehouseRequisition.Repositories;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Services;

/// <summary>
/// Business rules for the requisition workflow live here; controllers stay thin.
/// </summary>
public class RequisitionService : IRequisitionService
{
    private readonly IRequisitionRepository _repository;
    private readonly ICatalogService _catalogService;
    private readonly IPartService _partService;
    private readonly ICurrentUserService _currentUser;

    public RequisitionService(
        IRequisitionRepository repository,
        ICatalogService catalogService,
        IPartService partService,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _catalogService = catalogService;
        _partService = partService;
        _currentUser = currentUser;
    }

    public DashboardViewModel GetDashboard()
    {
        var requisitions = _repository.GetAll().ToList();

        var viewModel = new DashboardViewModel();
        foreach (RequisitionStatus status in Enum.GetValues<RequisitionStatus>())
        {
            viewModel.Counts[status] = requisitions.Count(r => r.Status == status);
        }

        viewModel.RecentRequisitions = requisitions
            .OrderByDescending(r => r.CreatedAt)
            .Take(8)
            .Select(MapListItem)
            .ToList();

        viewModel.AttentionRequisitions = requisitions
            .Where(r => r.Status == RequisitionStatus.InProgress)
            .OrderBy(r => r.StartedAt ?? r.CreatedAt)
            .Take(5)
            .Select(MapListItem)
            .ToList();

        return viewModel;
    }

    public List<ActivityEntryViewModel> GetRecentActivity(int maxEntries = 8)
    {
        var entries = new List<ActivityEntryViewModel>();
        var plants = _catalogService.GetPlants().ToDictionary(p => p.Id, p => p.Name);

        foreach (var requisition in _repository.GetAll())
        {
            entries.Add(new ActivityEntryViewModel
            {
                Icon = StatusText.IconFor(RequisitionStatus.Open),
                Tone = "open",
                Title = $"{requisition.RequisitionNumber} fue creada",
                Description = $"Por {requisition.RequesterName}",
                Timestamp = requisition.CreatedAt
            });

            if (requisition.Status is not RequisitionStatus.Open && requisition.StartedAt.HasValue)
            {
                entries.Add(new ActivityEntryViewModel
                {
                    Icon = StatusText.IconFor(RequisitionStatus.InProgress),
                    Tone = "progress",
                    Title = $"{requisition.RequisitionNumber} está en proceso",
                    Description = $"Almacén inició el surtido en {plants.GetValueOrDefault(requisition.PlantId, "planta")}",
                    Timestamp = requisition.StartedAt.Value
                });
            }

            if (requisition.ClosedAt.HasValue)
            {
                entries.Add(new ActivityEntryViewModel
                {
                    Icon = StatusText.IconFor(requisition.Status),
                    Tone = StatusText.ToneFor(requisition.Status),
                    Title = $"{requisition.RequisitionNumber} · {StatusText.For(requisition.Status)}",
                    Description = string.IsNullOrWhiteSpace(requisition.ClosedBy)
                        ? "Movida al historial"
                        : $"Cerrada por {requisition.ClosedBy}",
                    Timestamp = requisition.ClosedAt.Value
                });
            }
        }

        return entries
            .OrderByDescending(e => e.Timestamp)
            .Take(maxEntries)
            .ToList();
    }

    public RequisitionListViewModel GetPendingList(RequisitionFilterViewModel filter)
    {
        filter.Status = "pending";
        return BuildList(filter, isHistory: false);
    }

    public RequisitionListViewModel GetHistory(RequisitionFilterViewModel filter)
    {
        if (!IsTerminalStatusKey(filter.Status))
        {
            // History only ever shows completed requisitions.
            filter.Status = "all-terminal";
        }

        return BuildList(filter, isHistory: true);
    }

    public RequisitionDetailsViewModel? GetDetails(int id)
    {
        var requisition = _repository.GetById(id);
        return requisition is null ? null : MapDetails(requisition);
    }

    public CreateRequisitionViewModel GetNewRequisitionTemplate() => new()
    {
        RequestedDate = DateTime.Today,
        Items = []
    };

    public OperationResult<Requisition> CreateRequisition(CreateRequisitionViewModel model)
    {
        if (model.Items.Count == 0)
        {
            return OperationResult<Requisition>.Fail("Debes agregar al menos un material.");
        }

        foreach (var item in model.Items)
        {
            if (string.IsNullOrWhiteSpace(item.PartNumber))
            {
                return OperationResult<Requisition>.Fail("El número de parte es obligatorio.");
            }

            if (item.RequestedQuantity <= 0)
            {
                return OperationResult<Requisition>.Fail(
                    $"La cantidad solicitada de {item.PartNumber.Trim().ToUpperInvariant()} debe ser mayor que cero.");
            }
        }

        var requisitionNumber = _repository.GetNextRequisitionNumber(model.RequestedDate.Date);
        var requesterId = ResolveRequesterId(model.EmployeeNumber, model.RequesterName);

        var requisition = new Requisition
        {
            RequisitionNumber = requisitionNumber,
            CreatedAt = DateTime.Now,
            RequestedDate = model.RequestedDate.Date,
            RequesterId = requesterId,
            RequesterName = model.RequesterName.Trim(),
            EmployeeNumber = model.EmployeeNumber.Trim(),
            PlantId = model.PlantId,
            AreaId = model.AreaId,
            MachineId = model.MachineId,
            Status = RequisitionStatus.Open,
            CreatedBy = _currentUser.DisplayName,
            Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim(),
            Items = []
        };

        foreach (var input in model.Items)
        {
            var part = _partService.FindByPartNumber(input.PartNumber);
            requisition.Items.Add(new RequisitionItem
            {
                PartId = part?.Id ?? 0,
                PartNumber = input.PartNumber.Trim().ToUpperInvariant(),
                Description = !string.IsNullOrWhiteSpace(input.Description)
                    ? input.Description!.Trim()
                    : part?.Description ?? string.Empty,
                RequestedQuantity = input.RequestedQuantity,
                QuantityDescription = NullIfEmpty(input.QuantityDescription),
                UnitOfMeasure = string.IsNullOrWhiteSpace(input.UnitOfMeasure)
                    ? part?.UnitOfMeasure ?? "PZA"
                    : input.UnitOfMeasure.Trim(),
                Location = NullIfEmpty(input.Location) ?? part?.DefaultLocation,
                Observations = NullIfEmpty(input.Observations)
            });
        }

        _repository.Add(requisition);
        return OperationResult<Requisition>.Ok(requisition);
    }

    public OperationResult DeleteRequisition(int id)
    {
        var requisition = _repository.GetById(id);
        if (requisition is null)
        {
            return OperationResult.Fail("La requisición no existe.");
        }

        if (requisition.IsCompleted)
        {
            return OperationResult.Fail("No se puede eliminar una requisición completada.");
        }

        _repository.Delete(id);
        return OperationResult.Ok();
    }

    public FulfillmentViewModel? GetFulfillment(string requisitionNumber)
    {
        var requisition = _repository.GetByNumber(requisitionNumber);
        if (requisition is null)
        {
            return null;
        }

        if (requisition.Status == RequisitionStatus.Open)
        {
            // Business rule 9: opening an OPEN requisition moves it to IN_PROGRESS.
            requisition.Status = RequisitionStatus.InProgress;
            requisition.StartedAt ??= DateTime.Now;
            _repository.Update(requisition);
        }

        return new FulfillmentViewModel
        {
            RequisitionId = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            Status = requisition.Status,
            CreatedAt = requisition.CreatedAt,
            RequestedDate = requisition.RequestedDate,
            RequesterName = requisition.RequesterName,
            EmployeeNumber = requisition.EmployeeNumber,
            PlantName = _catalogService.GetPlants().FirstOrDefault(p => p.Id == requisition.PlantId)?.Name ?? "-",
            AreaName = _catalogService.GetAreas().FirstOrDefault(a => a.Id == requisition.AreaId)?.Name ?? "-",
            MachineName = _catalogService.GetMachines().FirstOrDefault(m => m.Id == requisition.MachineId)?.Name ?? "-",
            Items = requisition.Items.Select(i => new FulfillmentItemViewModel
            {
                Id = i.Id,
                PartNumber = i.PartNumber,
                Description = i.Description,
                UnitOfMeasure = i.UnitOfMeasure,
                RequestedQuantity = i.RequestedQuantity,
                QuantityDescription = i.QuantityDescription,
                FulfilledQuantity = i.FulfilledQuantity,
                Location = i.Location,
                Observations = i.Observations,
                ShortageReasonId = i.ShortageReasonId,
                ShortageComment = i.ShortageComment,
                Reviewed = i.Reviewed,
                FulfillmentStatus = ComputeItemStatus(i)
            }).ToList(),
            ShortageReasons = _catalogService.GetShortageReasons(activeOnly: false)
        };
    }

    public OperationResult<FulfillmentUpdateOutcome> UpdateFulfillmentItem(FulfillmentItemUpdateInput input)
    {
        var requisition = _repository.GetById(input.RequisitionId);
        if (requisition is null)
        {
            return OperationResult<FulfillmentUpdateOutcome>.Fail("La requisición no existe.");
        }

        if (requisition.IsCompleted)
        {
            return OperationResult<FulfillmentUpdateOutcome>.Fail("Esta requisición ya fue finalizada.");
        }

        var item = requisition.Items.FirstOrDefault(i => i.Id == input.ItemId);
        if (item is null)
        {
            return OperationResult<FulfillmentUpdateOutcome>.Fail("El material no existe en esta requisición.");
        }

        if (input.FulfilledQuantity < 0)
        {
            return OperationResult<FulfillmentUpdateOutcome>.Fail("La cantidad surtida no puede ser negativa.");
        }

        if (input.FulfilledQuantity > item.RequestedQuantity)
        {
            return OperationResult<FulfillmentUpdateOutcome>.Fail(
                "La cantidad surtida no puede ser mayor que la cantidad solicitada.");
        }

        ShortageReason? reason = null;
        if (input.ShortageReasonId.HasValue)
        {
            reason = _catalogService.GetShortageReasons(activeOnly: false)
                .FirstOrDefault(r => r.Id == input.ShortageReasonId.Value);
        }

        var status = ComputeItemStatus(item.RequestedQuantity, input.FulfilledQuantity);
        if (status is FulfillmentStatus.Partial or FulfillmentStatus.NotFulfilled)
        {
            if (reason is null)
            {
                return OperationResult<FulfillmentUpdateOutcome>.Fail(
                    "Debes seleccionar una razón para el faltante.");
            }

            if (reason.RequiresComment && string.IsNullOrWhiteSpace(input.ShortageComment))
            {
                return OperationResult<FulfillmentUpdateOutcome>.Fail(
                    "Debes especificar la razón del faltante.");
            }
        }

        item.FulfilledQuantity = input.FulfilledQuantity;
        item.FulfillmentStatus = status;

        if (status is FulfillmentStatus.Partial or FulfillmentStatus.NotFulfilled && reason is not null)
        {
            item.ShortageReasonId = reason.Id;
            item.ShortageReasonDescription = reason.Description;
            item.ShortageComment = NullIfEmpty(input.ShortageComment);
        }
        else
        {
            item.ShortageReasonId = null;
            item.ShortageReasonDescription = null;
            item.ShortageComment = null;
        }

        if (input.Reviewed)
        {
            item.Reviewed = true;
            item.ReviewedAt = DateTime.Now;
        }
        else
        {
            // The user reopened the material to edit it.
            item.Reviewed = false;
            item.ReviewedAt = null;
        }

        _repository.Update(requisition);

        return OperationResult<FulfillmentUpdateOutcome>.Ok(new FulfillmentUpdateOutcome
        {
            ItemId = item.Id,
            FulfillmentStatus = item.FulfillmentStatus,
            Reviewed = item.Reviewed,
            ReviewedItems = requisition.Items.Count(i => i.Reviewed),
            TotalItems = requisition.Items.Count
        });
    }

    public OperationResult<string> FinalizeRequisition(string requisitionNumber)
    {
        var requisition = _repository.GetByNumber(requisitionNumber);
        if (requisition is null)
        {
            return OperationResult<string>.Fail("La requisición no existe.");
        }

        if (requisition.IsCompleted)
        {
            return OperationResult<string>.Fail("Esta requisición ya fue finalizada.");
        }

        if (requisition.Items.Any(i => !i.Reviewed))
        {
            return OperationResult<string>.Fail("Debes revisar todos los materiales antes de finalizar.");
        }

        foreach (var item in requisition.Items)
        {
            if (item.FulfilledQuantity < 0 || item.FulfilledQuantity > item.RequestedQuantity)
            {
                return OperationResult<string>.Fail(
                    $"La cantidad surtida de {item.PartNumber} no es válida.");
            }

            var hasShortage = item.FulfilledQuantity < item.RequestedQuantity;
            if (hasShortage && item.ShortageReasonId is null)
            {
                return OperationResult<string>.Fail(
                    $"Debes seleccionar una razón para el faltante de {item.PartNumber}.");
            }

            var requiresComment = item.ShortageReasonId.HasValue &&
                                  _catalogService.GetShortageReasons(activeOnly: false)
                                      .FirstOrDefault(r => r.Id == item.ShortageReasonId.Value)?.RequiresComment == true;

            if (requiresComment && string.IsNullOrWhiteSpace(item.ShortageComment))
            {
                return OperationResult<string>.Fail(
                    $"Debes especificar la razón del faltante de {item.PartNumber}.");
            }
        }

        var allComplete = requisition.Items.All(i =>
            ComputeItemStatus(i.RequestedQuantity, i.FulfilledQuantity) == FulfillmentStatus.Complete);

        requisition.Status = allComplete
            ? RequisitionStatus.Fulfilled
            : RequisitionStatus.PartiallyFulfilled;
        requisition.ClosedAt = DateTime.Now;
        requisition.ClosedBy = _currentUser.DisplayName;

        _repository.Update(requisition);

        return OperationResult<string>.Ok(StatusText.For(requisition.Status));
    }

    private RequisitionListViewModel BuildList(RequisitionFilterViewModel filter, bool isHistory)
    {
        var requisitions = _repository.GetAll();
        var filtered = ApplyFilter(requisitions, filter, isHistory);

        return new RequisitionListViewModel
        {
            Filter = filter,
            IsHistory = isHistory,
            Plants = _catalogService.GetPlants(),
            Areas = _catalogService.GetAreas(),
            Requisitions = filtered.Select(MapListItem).ToList()
        };
    }

    private IEnumerable<Requisition> ApplyFilter(IEnumerable<Requisition> source, RequisitionFilterViewModel filter, bool isHistory)
    {
        IEnumerable<Requisition> query = source;

        switch (filter.Status)
        {
            case "pending":
                query = query.Where(r => !r.IsCompleted);
                break;
            case "all":
                break;
            case "all-terminal":
                query = query.Where(r => r.IsCompleted);
                break;
            default:
                if (Enum.TryParse<RequisitionStatus>(filter.Status, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
                else if (isHistory)
                {
                    query = query.Where(r => r.IsCompleted);
                }
                else
                {
                    query = query.Where(r => !r.IsCompleted);
                }

                break;
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt.Date >= filter.FromDate.Value.Date);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt.Date <= filter.ToDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(filter.Requester))
        {
            var requester = filter.Requester.Trim();
            query = query.Where(r => r.RequesterName.Contains(requester, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.PlantId.HasValue)
        {
            query = query.Where(r => r.PlantId == filter.PlantId.Value);
        }

        if (filter.AreaId.HasValue)
        {
            query = query.Where(r => r.AreaId == filter.AreaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(r =>
                r.RequisitionNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.RequesterName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.Items.Any(i =>
                    i.PartNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    i.Description.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        return filter.Sort switch
        {
            "date_asc" => query.OrderBy(r => r.CreatedAt),
            "number_asc" => query.OrderBy(r => r.RequisitionNumber),
            "number_desc" => query.OrderByDescending(r => r.RequisitionNumber),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };
    }

    private RequisitionListItemViewModel MapListItem(Requisition requisition)
    {
        var plants = _catalogService.GetPlants();
        var areas = _catalogService.GetAreas();
        var machines = _catalogService.GetMachines();

        return new RequisitionListItemViewModel
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            CreatedAt = requisition.CreatedAt,
            RequesterName = requisition.RequesterName,
            EmployeeNumber = requisition.EmployeeNumber,
            PlantName = plants.FirstOrDefault(p => p.Id == requisition.PlantId)?.Name ?? "-",
            AreaName = areas.FirstOrDefault(a => a.Id == requisition.AreaId)?.Name ?? "-",
            MachineName = machines.FirstOrDefault(m => m.Id == requisition.MachineId)?.Code ?? "-",
            Status = requisition.Status,
            TotalItems = requisition.Items.Count,
            ReviewedItems = requisition.Items.Count(i => i.Reviewed),
            ShortageCount = requisition.Items.Count(i => i.ShortageQuantity > 0),
            ClosedAt = requisition.ClosedAt,
            ClosedByName = requisition.ClosedBy
        };
    }

    private static RequisitionDetailsViewModel MapDetails(Requisition requisition) => new()
    {
        Id = requisition.Id,
        RequisitionNumber = requisition.RequisitionNumber,
        CreatedAt = requisition.CreatedAt,
        RequestedDate = requisition.RequestedDate,
        RequesterName = requisition.RequesterName,
        EmployeeNumber = requisition.EmployeeNumber,
        Status = requisition.Status,
        Notes = requisition.Notes,
        Items = requisition.Items.Select(i => new RequisitionDetailsItemViewModel
        {
            Id = i.Id,
            PartNumber = i.PartNumber,
            Description = i.Description,
            RequestedQuantity = i.RequestedQuantity,
            FulfilledQuantity = i.FulfilledQuantity,
            UnitOfMeasure = i.UnitOfMeasure,
            Location = i.Location,
            FulfillmentStatus = ComputeItemStatus(i),
            Reviewed = i.Reviewed
        }).ToList()
    };

    private static FulfillmentStatus ComputeItemStatus(RequisitionItem item) =>
        ComputeItemStatus(item.RequestedQuantity, item.FulfilledQuantity);

    private static FulfillmentStatus ComputeItemStatus(decimal requested, decimal fulfilled) =>
        fulfilled switch
        {
            var quantity when quantity >= requested => FulfillmentStatus.Complete,
            > 0 => FulfillmentStatus.Partial,
            _ => FulfillmentStatus.NotFulfilled
        };

    private int ResolveRequesterId(string employeeNumber, string requesterName)
    {
        var users = _catalogService.GetUsers();
        var match = users.FirstOrDefault(u =>
            string.Equals(u.EmployeeNumber, employeeNumber.Trim(), StringComparison.OrdinalIgnoreCase)) ??
                     users.FirstOrDefault(u =>
                         u.Name.Equals(requesterName.Trim(), StringComparison.OrdinalIgnoreCase));

        return match?.Id ?? 0;
    }

    private static bool IsTerminalStatusKey(string status)
    {
        switch (status)
        {
            case "all":
            case "all-terminal":
                return true;
            default:
                return Enum.TryParse<RequisitionStatus>(status, out var parsed) && parsed
                    is RequisitionStatus.Fulfilled
                    or RequisitionStatus.PartiallyFulfilled
                    or RequisitionStatus.Closed
                    or RequisitionStatus.Cancelled;
        }
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
