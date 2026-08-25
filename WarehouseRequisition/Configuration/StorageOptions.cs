namespace WarehouseRequisition.Configuration;

public class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Relative path of the JSON snapshot file used by the prototype.</summary>
    public string FilePath { get; set; } = "App_Data/datastore.json";
}

public class FulfillmentOptions
{
    public const string SectionName = "Fulfillment";

    /// <summary>
    /// Base URL encoded inside QR codes, e.g. http://192.168.1.50:5000.
    /// When empty the absolute URL is derived from the incoming HTTP request.
    /// Configure this for deployment inside the factory network.
    /// </summary>
    public string? PublicBaseUrl { get; set; }
}
