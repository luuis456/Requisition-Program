using WarehouseRequisition.Enums;
using WarehouseRequisition.Models;

namespace WarehouseRequisition.ViewModels;

public class CatalogIndexViewModel
{
    public string ActiveTab { get; set; } = "parts";

    public List<Part> Parts { get; set; } = [];

    public List<Plant> Plants { get; set; } = [];

    public List<Area> Areas { get; set; } = [];

    public List<Machine> Machines { get; set; } = [];

    public List<User> Users { get; set; } = [];

    public List<ShortageReason> ShortageReasons { get; set; } = [];
}

public class DashboardViewModel
{
    public Dictionary<RequisitionStatus, int> Counts { get; set; } = new();

    public List<RequisitionListItemViewModel> RecentRequisitions { get; set; } = [];

    public List<RequisitionListItemViewModel> AttentionRequisitions { get; set; } = [];

    /// <summary>Alias kept for readability in views.</summary>
    public int TotalRequisitions => Counts.Values.Sum();
}
