using WarehouseRequisition.Enums;

namespace WarehouseRequisition.ViewModels;

public class RequisitionListItemViewModel
{
    public int Id { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string RequesterName { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public string AreaName { get; set; } = string.Empty;

    public string PlantName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public RequisitionStatus Status { get; set; }

    public int TotalItems { get; set; }

    public int ReviewedItems { get; set; }

    public int ShortageCount { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? ClosedByName { get; set; }
}

public class RequisitionListViewModel
{
    public List<RequisitionListItemViewModel> Requisitions { get; set; } = [];

    public RequisitionFilterViewModel Filter { get; set; } = new();

    public List<Models.Plant> Plants { get; set; } = [];

    public List<Models.Area> Areas { get; set; } = [];

    /// <summary>True when rendering /Requisitions/History (extra columns, different title).</summary>
    public bool IsHistory { get; set; }

    public string StatusOptionsKey => IsHistory ? "history" : "pending";
}
