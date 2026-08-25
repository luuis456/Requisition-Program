using WarehouseRequisition.Models;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Services;

public interface ICatalogService
{
    List<Plant> GetPlants();

    List<Area> GetAreas(int? plantId = null);

    List<Machine> GetMachines(int? areaId = null);

    List<User> GetUsers();

    List<ShortageReason> GetShortageReasons(bool activeOnly = true);
}
