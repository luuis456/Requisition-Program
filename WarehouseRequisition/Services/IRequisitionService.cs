using WarehouseRequisition.Common;
using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Services;

public class FulfillmentUpdateOutcome
{
    public int ItemId { get; set; }

    public Enums.FulfillmentStatus FulfillmentStatus { get; set; }

    public bool Reviewed { get; set; }

    public int ReviewedItems { get; set; }

    public int TotalItems { get; set; }
}

public interface IRequisitionService
{
    DashboardViewModel GetDashboard();

    List<ActivityEntryViewModel> GetRecentActivity(int maxEntries = 8);

    RequisitionListViewModel GetPendingList(RequisitionFilterViewModel filter);

    RequisitionListViewModel GetHistory(RequisitionFilterViewModel filter);

    RequisitionDetailsViewModel? GetDetails(int id);

    CreateRequisitionViewModel GetNewRequisitionTemplate();

    OperationResult<Models.Requisition> CreateRequisition(CreateRequisitionViewModel model);

    OperationResult DeleteRequisition(int id);

    /// <summary>Returns the fulfillment screen data; opening an OPEN requisition moves it to IN_PROGRESS.</summary>
    FulfillmentViewModel? GetFulfillment(string requisitionNumber);

    OperationResult<FulfillmentUpdateOutcome> UpdateFulfillmentItem(FulfillmentItemUpdateInput input);

    /// <summary>Finalizes the requisition; returns the Spanish label of the resulting status.</summary>
    OperationResult<string> FinalizeRequisition(string requisitionNumber);
}
