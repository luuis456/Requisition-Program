using WarehouseRequisition.Enums;

namespace WarehouseRequisition.Services;

public interface IBarcodeService
{
    /// <summary>
    /// Renders a linear (1D) barcode as standalone PNG bytes,
    /// mirroring IQrCodeService.GeneratePng.
    /// </summary>
    byte[] GeneratePng(string content, BarcodeSymbology symbology = BarcodeSymbology.Code128, int height = 90);
}
