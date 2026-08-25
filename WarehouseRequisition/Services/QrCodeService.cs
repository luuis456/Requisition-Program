using QRCoder;

namespace WarehouseRequisition.Services;

public class QrCodeService : IQrCodeService
{
    public byte[] GeneratePng(string content, int pixelsPerModule = 10)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        using var pngCode = new PngByteQRCode(data);
        return pngCode.GetGraphic(pixelsPerModule);
    }
}
