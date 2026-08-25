namespace WarehouseRequisition.ViewModels;

/// <summary>Filter toolbar state shared by the pending list and the history.</summary>
public class RequisitionFilterViewModel
{
    /// <summary>"pending" (Open + InProgress), "all", or a numeric RequisitionStatus value.</summary>
    public string Status { get; set; } = "pending";

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Requester { get; set; }

    public int? PlantId { get; set; }

    public int? AreaId { get; set; }

    public string? SearchTerm { get; set; }

    /// <summary>date_asc | date_desc | number_asc | number_desc</summary>
    public string Sort { get; set; } = "date_desc";

    public bool HasActiveFilters =>
        Status != "pending" ||
        FromDate.HasValue ||
        ToDate.HasValue ||
        !string.IsNullOrWhiteSpace(Requester) ||
        PlantId.HasValue ||
        AreaId.HasValue ||
        !string.IsNullOrWhiteSpace(SearchTerm);
}
