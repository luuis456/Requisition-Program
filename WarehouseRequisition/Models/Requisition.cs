using WarehouseRequisition.Enums;

namespace WarehouseRequisition.Models;

public class Requisition
{
    public int Id { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime RequestedDate { get; set; }

    public int RequesterId { get; set; }

    public string RequesterName { get; set; } = string.Empty;

    public string EmployeeNumber { get; set; } = string.Empty;

    public int PlantId { get; set; }

    public int AreaId { get; set; }

    public int MachineId { get; set; }

    public RequisitionStatus Status { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? StartedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? ClosedBy { get; set; }

    public string? Notes { get; set; }

    public List<RequisitionItem> Items { get; set; } = [];

    public bool IsCompleted =>
        Status is RequisitionStatus.Fulfilled
            or RequisitionStatus.PartiallyFulfilled
            or RequisitionStatus.Closed
            or RequisitionStatus.Cancelled;
}
