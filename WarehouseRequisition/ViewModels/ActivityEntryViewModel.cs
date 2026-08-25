namespace WarehouseRequisition.ViewModels;

public class ActivityEntryViewModel
{
    public string Icon { get; set; } = "bi-activity";

    public string Tone { get; set; } = "open";

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}
