using Microsoft.AspNetCore.Mvc;
using WarehouseRequisition.Common;
using WarehouseRequisition.Configuration;
using WarehouseRequisition.Services;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Controllers;

public class RequisitionsController : Controller
{
    private readonly IRequisitionService _requisitionService;
    private readonly ICatalogService _catalogService;
    private readonly IMaterialGenerationService _materialGenerationService;
    private readonly IQrCodeService _qrCodeService;
    private readonly FulfillmentOptions _fulfillmentOptions;

    public RequisitionsController(
        IRequisitionService requisitionService,
        ICatalogService catalogService,
        IMaterialGenerationService materialGenerationService,
        IQrCodeService qrCodeService,
        Microsoft.Extensions.Options.IOptions<FulfillmentOptions> fulfillmentOptions)
    {
        _requisitionService = requisitionService;
        _catalogService = catalogService;
        _materialGenerationService = materialGenerationService;
        _qrCodeService = qrCodeService;
        _fulfillmentOptions = fulfillmentOptions.Value;
    }

    public IActionResult Index([FromQuery] RequisitionFilterViewModel filter)
    {
        ViewData["Title"] = "Requisiciones pendientes";
        ViewData["ActiveSection"] = "pending";
        return View(_requisitionService.GetPendingList(filter));
    }

    public IActionResult History([FromQuery] RequisitionFilterViewModel filter)
    {
        ViewData["Title"] = "Historial";
        ViewData["ActiveSection"] = "history";
        return View("History", _requisitionService.GetHistory(filter));
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nueva requisición";
        ViewData["ActiveSection"] = "create";
        return View(BuildCreateViewModelWithCatalogs(_requisitionService.GetNewRequisitionTemplate()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateRequisitionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Nueva requisición";
            ViewData["ActiveSection"] = "create";
            ViewBag.FormErrors = CollectErrors(ModelState);
            return View(BuildCreateViewModelWithCatalogs(model));
        }

        var result = _requisitionService.CreateRequisition(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            ViewData["Title"] = "Nueva requisición";
            ViewData["ActiveSection"] = "create";
            ViewBag.FormErrors = new List<string> { result.ErrorMessage! };
            return View(BuildCreateViewModelWithCatalogs(model));
        }

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = $"Requisición {result.Value!.RequisitionNumber} creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var result = _requisitionService.DeleteRequisition(id);
        TempData["ToastType"] = result.Success ? "success" : "error";
        TempData["ToastMessage"] = result.Success
            ? "La requisición fue eliminada."
            : result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Returns the details modal as a partial view, loaded via AJAX.</summary>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var viewModel = _requisitionService.GetDetails(id);
        if (viewModel is null)
        {
            return NotFound();
        }

        viewModel.FulfillmentUrl = Url.Action("Index", "Fulfillment", new { requisitionNumber = viewModel.RequisitionNumber })!;
        viewModel.QrImageUrl = Url.Action("QrCode", "Requisitions", new { requisitionNumber = viewModel.RequisitionNumber })!;
        return PartialView("_DetailsModal", viewModel);
    }

    /// <summary>Renders the QR code image pointing to the mobile fulfillment URL.</summary>
    [HttpGet]
    public IActionResult QrCode(string requisitionNumber, bool download = false)
    {
        var fulfillmentPath = Url.Action("Index", "Fulfillment", new { requisitionNumber })!;
        var content = string.IsNullOrWhiteSpace(_fulfillmentOptions.PublicBaseUrl)
            ? $"{Request.Scheme}://{Request.Host}{fulfillmentPath}"
            : $"{_fulfillmentOptions.PublicBaseUrl.TrimEnd('/')}{fulfillmentPath}";

        var bytes = _qrCodeService.GeneratePng(content);
        return download
            ? File(bytes, "image/png", $"QR-{requisitionNumber}.png")
            : File(bytes, "image/png");
    }

    /// <summary>Mock BOM/ERP integration endpoint used by the create screen.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AutoGenerateMaterials([FromBody] AutoGenerateMaterialsRequest request)
    {
        if (!ModelState.IsValid)
        {
            var firstError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Datos inválidos.";

            return BadRequest(new { success = false, message = firstError });
        }

        var items = _materialGenerationService.Generate(request);
        return Json(new { success = true, items });
    }

    private CreateRequisitionViewModel BuildCreateViewModelWithCatalogs(CreateRequisitionViewModel model)
    {
        // Catalogs are exposed to the view through ViewBag lists used by the select inputs.
        ViewBag.Plants = _catalogService.GetPlants();
        ViewBag.Areas = _catalogService.GetAreas();
        ViewBag.Machines = _catalogService.GetMachines();
        return model;
    }

    private static List<string> CollectErrors(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state) =>
        state.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Valor inválido." : e.ErrorMessage)
            .Distinct()
            .ToList();
}
