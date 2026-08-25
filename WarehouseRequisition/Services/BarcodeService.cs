using System.IO.Compression;
using WarehouseRequisition.Enums;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace WarehouseRequisition.Services;

/// <summary>
/// Renders linear (1D) barcodes as PNG images using ZXing.Net pixel output plus a
/// small built-in grayscale PNG encoder — pure managed code with no dependency on
/// System.Drawing, so it behaves identically on Windows, Linux and macOS.
/// Swap or extend here if label printing needs fonts or custom layouts later.
/// </summary>
public class BarcodeService : IBarcodeService
{
    public byte[] GeneratePng(string content, BarcodeSymbology symbology = BarcodeSymbology.Code128, int height = 90)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Barcode content must not be empty.", nameof(content));
        }

        content = content.Trim();

        if (content.Any(c => c > 126))
        {
            throw new ArgumentException("Barcode content supports ASCII characters only.", nameof(content));
        }

        ValidateSymbologyContent(content, symbology);

        var writer = new BarcodeWriterPixelData
        {
            Format = MapFormat(symbology),
            Options = new EncodingOptions
            {
                Height = Math.Clamp(height, 40, 400),
                Margin = 12,
                PureBarcode = true
            }
        };

        var pixels = writer.Write(content);
        return GrayscalePngEncoder.Encode(pixels.Width, pixels.Height, ExtractLuminance(pixels));
    }

    private static BarcodeFormat MapFormat(BarcodeSymbology symbology) => symbology switch
    {
        BarcodeSymbology.Code39 => BarcodeFormat.CODE_39,
        BarcodeSymbology.Ean13 => BarcodeFormat.EAN_13,
        BarcodeSymbology.Ean8 => BarcodeFormat.EAN_8,
        BarcodeSymbology.UpcA => BarcodeFormat.UPC_A,
        _ => BarcodeFormat.CODE_128
    };

    private static void ValidateSymbologyContent(string content, BarcodeSymbology symbology)
    {
        switch (symbology)
        {
            case BarcodeSymbology.Ean13:
            case BarcodeSymbology.Ean8:
            case BarcodeSymbology.UpcA:
                var requiredLength = symbology switch
                {
                    BarcodeSymbology.Ean13 => 12, // check digit is calculated automatically.
                    BarcodeSymbology.Ean8 => 7,
                    _ => 11
                };
                if (content.Length != requiredLength || content.Any(c => c is < '0' or > '9'))
                {
                    throw new ArgumentException(
                        $"{symbology} requires exactly {requiredLength} digits (without the check digit).");
                }
                break;

            case BarcodeSymbology.Code39 when content.Any(c => !"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-. $/+%".Contains(c)):
                throw new ArgumentException("Code 39 supports A-Z, digits, space and - . $ / + % only.");
        }
    }

    private static byte[] ExtractLuminance(PixelData pixels)
    {
        // ZXing PixelData stores 4 bytes per pixel (BGRA). Barcodes are pure black
        // and white, so every channel holds the same value: keep one byte per pixel.
        var luminance = new byte[pixels.Width * pixels.Height];
        for (var i = 0; i < luminance.Length; i++)
        {
            luminance[i] = pixels.Pixels[i * 4];
        }

        return luminance;
    }
}

/// <summary>
/// Minimal PNG writer for 8-bit grayscale images (filter type 0), enough for
/// black-and-white barcode rendering without pulling in an imaging library.
/// </summary>
internal static class GrayscalePngEncoder
{
    private static readonly uint[] CrcTable = BuildCrcTable();

    public static byte[] Encode(int width, int height, byte[] grayPixels)
    {
        var stride = width + 1; // one leading filter-type byte (0) per scanline
        var raw = new byte[stride * height];
        for (var y = 0; y < height; y++)
        {
            Buffer.BlockCopy(grayPixels, y * width, raw, y * stride + 1, width);
        }

        using var output = new MemoryStream();

        output.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]); // PNG signature

        var header = new byte[13];
        WriteBigEndian(header.AsSpan(0), (uint)width);
        WriteBigEndian(header.AsSpan(4), (uint)height);
        header[8] = 8;  // bit depth
        header[9] = 0;  // color type: grayscale
        // compression (0), filter (0) and interlace (0) are already zero.
        WriteChunk(output, "IHDR", header);

        var idat = CompressZlib(raw);
        WriteChunk(output, "IDAT", idat);

        WriteChunk(output, "IEND", ReadOnlySpan<byte>.Empty);

        return output.ToArray();
    }

    private static byte[] CompressZlib(byte[] data)
    {
        using var buffer = new MemoryStream();
        buffer.WriteByte(0x78); // CMF: deflate, 32K window
        buffer.WriteByte(0x01); // FLG: no dictionary, fastest check bits

        using (var deflate = new DeflateStream(buffer, CompressionLevel.Optimal, leaveOpen: true))
        {
            deflate.Write(data, 0, data.Length);
        }

        WriteBigEndianToStream(buffer, Adler32(data));
        return buffer.ToArray();
    }

    private static void WriteChunk(Stream stream, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> lengthPrefix = stackalloc byte[4];
        WriteBigEndian(lengthPrefix, (uint)data.Length);
        stream.Write(lengthPrefix);

        var typeBytes = new byte[4];
        for (var i = 0; i < 4; i++)
        {
            typeBytes[i] = (byte)type[i];
        }

        stream.Write(typeBytes);
        stream.Write(data);

        Span<byte> crcBytes = stackalloc byte[4];
        WriteBigEndian(crcBytes, Crc32(typeBytes, data));
        stream.Write(crcBytes);
    }

    private static void WriteBigEndian(Span<byte> target, uint value)
    {
        target[0] = (byte)(value >> 24);
        target[1] = (byte)(value >> 16);
        target[2] = (byte)(value >> 8);
        target[3] = (byte)value;
    }

    private static void WriteBigEndianToStream(Stream stream, uint value)
    {
        stream.WriteByte((byte)(value >> 24));
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }

    private static uint Crc32(byte[] typeBytes, ReadOnlySpan<byte> data)
    {
        uint crc = 0xFFFFFFFF;
        foreach (var b in typeBytes)
        {
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        foreach (var b in data)
        {
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        return crc ^ 0xFFFFFFFF;
    }

    private static uint Adler32(byte[] data)
    {
        uint a = 1, b = 0;
        foreach (var byteValue in data)
        {
            a = (a + byteValue) % 65521;
            b = (b + a) % 65521;
        }

        return (b << 16) | a;
    }

    private static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            var c = n;
            for (var k = 0; k < 8; k++)
            {
                c = (c & 1) == 1 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }
}
