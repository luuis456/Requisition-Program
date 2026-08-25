using WarehouseRequisition.Enums;

namespace WarehouseRequisition.Models;

public class RequisitionItem
{
    public int Id { get; set; }

    public int RequisitionId { get; set; }

    public int PartId { get; set; }

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal RequestedQuantity { get; set; }

    public string? QuantityDescription { get; set; }

    public string UnitOfMeasure { get; set; } = string.Empty;

    public string? Location { get; set; }

    public string? Observations { get; set; }

    public decimal FulfilledQuantity { get; set; }

    public decimal ShortageQuantity => Math.Max(0, RequestedQuantity - FulfilledQuantity);

    public FulfillmentStatus FulfillmentStatus { get; set; }

    public int? ShortageReasonId { get; set; }

    public string? ShortageReasonDescription { get; set; }

    public string? ShortageComment { get; set; }

    public bool Reviewed { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
