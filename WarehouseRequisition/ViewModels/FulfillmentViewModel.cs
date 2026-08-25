using WarehouseRequisition.Enums;

namespace WarehouseRequisition.ViewModels;

public class FulfillmentItemViewModel
{
    public int Id { get; set; }

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string UnitOfMeasure { get; set; } = string.Empty;

    public decimal RequestedQuantity { get; set; }

    public string? QuantityDescription { get; set; }

    public decimal FulfilledQuantity { get; set; }

    public string? Location { get; set; }

    public string? Observations { get; set; }

    public int? ShortageReasonId { get; set; }

    public string? ShortageComment { get; set; }

    public bool Reviewed { get; set; }

    public FulfillmentStatus FulfillmentStatus { get; set; }
}

public class FulfillmentViewModel
{
    public int RequisitionId { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public RequisitionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime RequestedDate { get; set; }

    public string RequesterName { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public string PlantName { get; set; } = string.Empty;

    public string AreaName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public List<FulfillmentItemViewModel> Items { get; set; } = [];

    public List<Models.ShortageReason> ShortageReasons { get; set; } = [];

    public int TotalItems => Items.Count;

    public int ReviewedItems => Items.Count(i => i.Reviewed);

    public int CompleteItems => Items.Count(i => i.FulfillmentStatus == FulfillmentStatus.Complete);

    public int ShortageItems => Items.Count(i =>
        i.FulfillmentStatus is FulfillmentStatus.Partial or FulfillmentStatus.NotFulfilled);
}
