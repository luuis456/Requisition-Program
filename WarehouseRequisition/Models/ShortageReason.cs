namespace WarehouseRequisition.Models;

public class ShortageReason
{
    public int Id { get; set; }

    /// <summary>Stable code (e.g. OUT_OF_STOCK) so the catalog can be migrated to a database later.</summary>
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool RequiresComment { get; set; }

    public bool IsActive { get; set; } = true;
}
