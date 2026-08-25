namespace WarehouseRequisition.Enums;

public enum BarcodeSymbology
{
    /// <summary>Code 128 — accepts any ASCII text; ideal for part numbers like MAT-10001.</summary>
    Code128,

    /// <summary>Code 39 — accepts A-Z, digits, space and - . $ / + %.</summary>
    Code39,

    /// <summary>EAN-13 — requires exactly 12 digits (the check digit is calculated automatically).</summary>
    Ean13,

    /// <summary>EAN-8 — requires exactly 7 digits (the check digit is calculated automatically).</summary>
    Ean8,

    /// <summary>UPC-A — requires exactly 11 digits (the check digit is calculated automatically).</summary>
    UpcA
}
