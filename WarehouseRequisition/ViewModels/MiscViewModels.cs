namespace WarehouseRequisition.ViewModels;

public class SettingsViewModel
{
    public string StorageMode { get; set; } = "Memoria + archivo JSON";

    public string DataFilePath { get; set; } = string.Empty;

    public bool DataFileExists { get; set; }

    public int RequisitionCount { get; set; }

    public int PartCount { get; set; }

    public int UserCount { get; set; }

    public string QrBaseUrl { get; set; } = "Automática (URL de la petición actual)";
}

public class EmptyStateViewModel
{
    public string Icon { get; set; } = "bi-inbox";

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}
