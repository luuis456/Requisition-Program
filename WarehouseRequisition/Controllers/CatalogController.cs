using Microsoft.AspNetCore.Mvc;
using WarehouseRequisition.Enums;
using WarehouseRequisition.Services;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly IPartService _partService;
    private readonly IBarcodeService _barcodeService;

    public CatalogController(ICatalogService catalogService, IPartService partService, IBarcodeService barcodeService)
    {
        _catalogService = catalogService;
        _partService = partService;
        _barcodeService = barcodeService;
    }

    public IActionResult Index(string tab = "parts")
    {
        ViewData["Title"] = "Catálogos";
        ViewData["ActiveSection"] = "catalog";

        var allowedTabs = new[] { "parts", "plants", "areas", "machines", "users", "shortage-reasons" };
        if (!allowedTabs.Contains(tab))
        {
            tab = "parts";
        }

        ViewBag.PlantNames = _catalogService.GetPlants().ToDictionary(p => p.Id, p => p.Name);
        ViewBag.AreaNames = _catalogService.GetAreas().ToDictionary(a => a.Id, a => a.Name);

        return View(new CatalogIndexViewModel
        {
            ActiveTab = tab,
            Parts = _partService.Search(null, maxResults: 500),
            Plants = _catalogService.GetPlants(),
            Areas = _catalogService.GetAreas(),
            Machines = _catalogService.GetMachines(),
            Users = _catalogService.GetUsers(),
            ShortageReasons = _catalogService.GetShortageReasons(activeOnly: false)
        });
    }

    /// <summary>Renders a linear barcode for a catalog part as a PNG image.</summary>
    [HttpGet]
    public IActionResult PartBarcode(string partNumber, BarcodeSymbology symbology = BarcodeSymbology.Code128, bool download = false)
    {
        var part = _partService.FindByPartNumber(partNumber ?? string.Empty);
        if (part is null)
        {
            return NotFound();
        }

        try
        {
            var bytes = _barcodeService.GeneratePng(part.PartNumber, symbology);
            return download
                ? File(bytes, "image/png", $"CB-{part.PartNumber}.png")
                : File(bytes, "image/png");
        }
        catch (ArgumentException ex)
        {
            // Symbology/content mismatch (e.g. EAN-13 with alphanumeric part number).
            return BadRequest(ex.Message);
        }
    }

    /// <summary>JSON lookup used by the create screen to auto-fill part data.</summary>
    [HttpGet]
    public IActionResult SearchParts(string term)
    {
        var part = _partService.FindByPartNumber(term ?? string.Empty);
        if (part is not null)
        {
            return Json(new
            {
                found = true,
                partNumber = part.PartNumber,
                description = part.Description,
                unitOfMeasure = part.UnitOfMeasure,
                defaultLocation = part.DefaultLocation
            });
        }

        var suggestions = _partService.Search(term, maxResults: 8)
            .Select(p => new
            {
                partNumber = p.PartNumber,
                description = p.Description,
                unitOfMeasure = p.UnitOfMeasure,
                defaultLocation = p.DefaultLocation
            });

        return Json(new { found = false, suggestions });
    }
}
