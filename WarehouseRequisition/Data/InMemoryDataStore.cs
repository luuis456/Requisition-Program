using WarehouseRequisition.Models;

namespace WarehouseRequisition.Data;

/// <summary>
/// Shared singleton state for the prototype. Designed so the collections map 1:1
/// to future EF Core entity sets when PostgreSQL is introduced.
/// </summary>
public class InMemoryDataStore
{
    /// <summary>Lock guarding every read-modify-write across repositories and services.</summary>
    public object SyncRoot { get; } = new();

    public List<User> Users { get; set; } = [];

    public List<Plant> Plants { get; set; } = [];

    public List<Area> Areas { get; set; } = [];

    public List<Machine> Machines { get; set; } = [];

    public List<Part> Parts { get; set; } = [];

    public List<PartLocation> PartLocations { get; set; } = [];

    public List<ShortageReason> ShortageReasons { get; set; } = [];

    public List<Requisition> Requisitions { get; set; } = [];
}
