namespace WarehouseRequisition.Services;

public interface IMaterialGenerationService
{
    /// <summary>
    /// Generates the material list for a production order.
    /// Mock implementation today; replace with the real BOM / ERP / SAP / AS400 integration later.
    /// </summary>
    List<ViewModels.RequisitionItemInputViewModel> Generate(ViewModels.AutoGenerateMaterialsRequest request);
}
