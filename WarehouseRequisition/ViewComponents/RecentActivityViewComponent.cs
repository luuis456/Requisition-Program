using Microsoft.AspNetCore.Mvc;
using WarehouseRequisition.Services;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.ViewComponents;

public class RecentActivityViewComponent : ViewComponent
{
    private readonly IRequisitionService _requisitionService;

    public RecentActivityViewComponent(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public IViewComponentResult Invoke(int maxEntries = 8)
    {
        var entries = _requisitionService.GetRecentActivity(maxEntries);
        return View(entries);
    }
}
