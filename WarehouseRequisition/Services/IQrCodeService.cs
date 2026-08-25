namespace WarehouseRequisition.Services;

public interface IQrCodeService
{
    /// <summary>Renders a QR code as PNG bytes.</summary>
    byte[] GeneratePng(string content, int pixelsPerModule = 10);
}
