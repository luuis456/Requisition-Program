using Microsoft.AspNetCore.Mvc;
using WarehouseRequisition.Common;
using WarehouseRequisition.Services;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Controllers;

/// <summary>
/// Mobile-first fulfillment screen reached by scanning the requisition QR code.
/// Attribute routes keep the friendly /Fulfillment/Index/REQ-YYYYMMDD-XXXX URLs.
/// </summary>
[Route("[controller]")]
public class FulfillmentController : Controller
{
    private readonly IRequisitionService _requisitionService;

    public FulfillmentController(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        TempData["ToastType"] = "info";
        TempData["ToastMessage"] = "Escanea el código QR de una requisición para comenzar a surtir.";
        return RedirectToAction("Index", "Requisitions");
    }

    [HttpGet("Index/{requisitionNumber}")]
    public IActionResult Index(string requisitionNumber)
    {
        var viewModel = _requisitionService.GetFulfillment(requisitionNumber);
        if (viewModel is null)
        {
            TempData["ToastType"] = "error";
            TempData["ToastMessage"] = $"No se encontró la requisición {requisitionNumber}.";
            return RedirectToAction("Index", "Requisitions");
        }

        ViewData["Title"] = $"Surtir {requisitionNumber}";
        ViewData["ActiveSection"] = "pending";
        return View(viewModel);
    }

    [HttpPost("UpdateItem")]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateItem([FromBody] FulfillmentItemUpdateInput input)
    {
        if (!ModelState.IsValid)
        {
            var firstError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Datos inválidos.";

            return BadRequest(new { success = false, message = firstError });
        }

        var result = _requisitionService.UpdateFulfillmentItem(input);
        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        var outcome = result.Value!;
        return Json(new
        {
            success = true,
            itemId = outcome.ItemId,
            reviewed = outcome.Reviewed,
            fulfillmentStatus = outcome.FulfillmentStatus.ToString(),
            fulfillmentStatusLabel = StatusText.For(outcome.FulfillmentStatus),
            fulfillmentStatusTone = StatusText.ToneFor(outcome.FulfillmentStatus),
            progressReviewed = outcome.ReviewedItems,
            progressTotal = outcome.TotalItems
        });
    }

    [HttpPost("{requisitionNumber}/Finalize")]
    [ValidateAntiForgeryToken]
    public IActionResult Finalize(string requisitionNumber)
    {
        var result = _requisitionService.FinalizeRequisition(requisitionNumber);
        if (!result.Success)
        {
            TempData["ToastType"] = "error";
            TempData["ToastMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index), new { requisitionNumber });
        }

        TempData["ToastType"] = result.Value == StatusText.For(Enums.RequisitionStatus.Fulfilled)
            ? "success"
            : "warning";

        TempData["ToastMessage"] = result.Value == StatusText.For(Enums.RequisitionStatus.Fulfilled)
            ? $"Requisición {requisitionNumber} surtada correctamente. Se movió al historial."
            : $"Requisición {requisitionNumber} finalizada como «{result.Value}». Se movió al historial.";

        return RedirectToAction("Index", "Requisitions");
    }
}