namespace WarehouseRequisition.Models;

public class Machine
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int AreaId { get; set; }

    public bool IsActive { get; set; } = true;
}
