using WarehouseRequisition.Models;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Services;

public interface IPartService
{
    Part? FindByPartNumber(string partNumber);

    List<Part> Search(string? term, int maxResults = 20);
}
