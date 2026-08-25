using WarehouseRequisition.Data;
using WarehouseRequisition.Models;

namespace WarehouseRequisition.Services;

public class CatalogService : ICatalogService
{
    private readonly InMemoryDataStore _store;

    public CatalogService(InMemoryDataStore store)
    {
        _store = store;
    }

    public List<Plant> GetPlants()
    {
        lock (_store.SyncRoot)
        {
            return _store.Plants.Where(p => p.IsActive).OrderBy(p => p.Name).ToList();
        }
    }

    public List<Area> GetAreas(int? plantId = null)
    {
        lock (_store.SyncRoot)
        {
            return _store.Areas
                .Where(a => a.IsActive && (!plantId.HasValue || a.PlantId == plantId.Value))
                .OrderBy(a => a.Name)
                .ToList();
        }
    }

    public List<Machine> GetMachines(int? areaId = null)
    {
        lock (_store.SyncRoot)
        {
            return _store.Machines
                .Where(m => m.IsActive && (!areaId.HasValue || m.AreaId == areaId.Value))
                .OrderBy(m => m.Code)
                .ToList();
        }
    }

    public List<User> GetUsers()
    {
        lock (_store.SyncRoot)
        {
            return _store.Users.OrderBy(u => u.Name).ToList();
        }
    }

    public List<ShortageReason> GetShortageReasons(bool activeOnly = true)
    {
        lock (_store.SyncRoot)
        {
            return _store.ShortageReasons
                .Where(r => !activeOnly || r.IsActive)
                .OrderBy(r => r.Id)
                .ToList();
        }
    }
}
