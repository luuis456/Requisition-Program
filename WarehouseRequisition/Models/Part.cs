namespace WarehouseRequisition.Models;

public class Part
{
    public int Id { get; set; }

    public string PartNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string UnitOfMeasure { get; set; } = string.Empty;

    public string DefaultLocation { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
