using Microsoft.AspNetCore.Mvc;
using WarehouseRequisition.Common;
using WarehouseRequisition.Configuration;
using WarehouseRequisition.Data;
using WarehouseRequisition.Services;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Controllers;

public class HomeController : Controller
{
    private readonly IRequisitionService _requisitionService;
    private readonly IPartService _partService;
    private readonly ICatalogService _catalogService;
    private readonly IDataStorePersistence _persistence;
    private readonly IDataStoreSeeder _seeder;
    private readonly FulfillmentOptions _fulfillmentOptions;

    public HomeController(
        IRequisitionService requisitionService,
        IPartService partService,
        ICatalogService catalogService,
        IDataStorePersistence persistence,
        IDataStoreSeeder seeder,
        Microsoft.Extensions.Options.IOptions<FulfillmentOptions> fulfillmentOptions)
    {
        _requisitionService = requisitionService;
        _partService = partService;
        _catalogService = catalogService;
        _persistence = persistence;
        _seeder = seeder;
        _fulfillmentOptions = fulfillmentOptions.Value;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Inicio";
        ViewData["ActiveSection"] = "home";
        return View(_requisitionService.GetDashboard());
    }

    public IActionResult Settings()
    {
        ViewData["Title"] = "Configuración";
        ViewData["ActiveSection"] = "settings";

        var viewModel = new SettingsViewModel
        {
            DataFilePath = _persistence.FilePath,
            DataFileExists = _persistence.Exists,
            QrBaseUrl = string.IsNullOrWhiteSpace(_fulfillmentOptions.PublicBaseUrl)
                ? "Automática (URL de la petición actual)"
                : _fulfillmentOptions.PublicBaseUrl
        };

        var dashboard = _requisitionService.GetDashboard();
        viewModel.RequisitionCount = dashboard.TotalRequisitions;
        viewModel.PartCount = _partService.Search(null, maxResults: 1000).Count;
        viewModel.UserCount = _catalogService.GetUsers().Count;
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetDemoData()
    {
        _seeder.ResetToSeedData();
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Los datos de demostración fueron restablecidos.";
        return RedirectToAction(nameof(Settings));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new Models.ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
