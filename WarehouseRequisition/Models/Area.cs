namespace WarehouseRequisition.Models;

public class Area
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int PlantId { get; set; }

    public bool IsActive { get; set; } = true;
}
