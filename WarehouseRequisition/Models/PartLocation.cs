namespace WarehouseRequisition.Models;

public class PartLocation
{
    public int Id { get; set; }

    public int PartId { get; set; }

    public string LocationCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
