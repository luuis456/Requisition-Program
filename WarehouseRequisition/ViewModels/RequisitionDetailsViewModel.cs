using WarehouseRequisition.Enums;

namespace WarehouseRequisition.ViewModels;

public class RequisitionDetailsItemViewModel
{
    public int Id { get; set; }

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal RequestedQuantity { get; set; }

    public decimal FulfilledQuantity { get; set; }

    public string UnitOfMeasure { get; set; } = string.Empty;

    public string? Location { get; set; }

    public FulfillmentStatus FulfillmentStatus { get; set; }

    public bool Reviewed { get; set; }
}

public class RequisitionDetailsViewModel
{
    public int Id { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime RequestedDate { get; set; }

    public string RequesterName { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public string AreaName { get; set; } = string.Empty;

    public string PlantName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public RequisitionStatus Status { get; set; }

    public string? Notes { get; set; }

    public List<RequisitionDetailsItemViewModel> Items { get; set; } = [];

    public int ReviewedItems => Items.Count(i => i.Reviewed);

    /// <summary>Relative URL of the endpoint that renders the QR image.</summary>
    public string QrImageUrl { get; set; } = string.Empty;

    /// <summary>Relative URL of the mobile fulfillment screen encoded in the QR code.</summary>
    public string FulfillmentUrl { get; set; } = string.Empty;
}
