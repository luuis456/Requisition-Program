using WarehouseRequisition.Enums;

namespace WarehouseRequisition.Common;

/// <summary>
/// Centralized Spanish (Mexico) display labels and badge tones for internal English enums.
/// Razor views must use these helpers instead of scattering translations.
/// </summary>
public static class StatusText
{
    public static string For(RequisitionStatus status) => status switch
    {
        RequisitionStatus.Open => "Abierta",
        RequisitionStatus.InProgress => "En proceso",
        RequisitionStatus.Fulfilled => "Surtida",
        RequisitionStatus.PartiallyFulfilled => "Surtida parcialmente",
        RequisitionStatus.Closed => "Cerrada",
        RequisitionStatus.Cancelled => "Cancelada",
        _ => status.ToString()
    };

    /// <summary>CSS modifier used by the shared status badge partial.</summary>
    public static string ToneFor(RequisitionStatus status) => status switch
    {
        RequisitionStatus.Open => "open",
        RequisitionStatus.InProgress => "progress",
        RequisitionStatus.Fulfilled => "fulfilled",
        RequisitionStatus.PartiallyFulfilled => "partial",
        RequisitionStatus.Closed => "closed",
        RequisitionStatus.Cancelled => "cancelled",
        _ => "closed"
    };

    public static string IconFor(RequisitionStatus status) => status switch
    {
        RequisitionStatus.Open => "bi-folder2-open",
        RequisitionStatus.InProgress => "bi-arrow-repeat",
        RequisitionStatus.Fulfilled => "bi-check2-circle",
        RequisitionStatus.PartiallyFulfilled => "bi-exclamation-circle",
        RequisitionStatus.Closed => "bi-lock",
        RequisitionStatus.Cancelled => "bi-x-circle",
        _ => "bi-folder"
    };

    public static string For(FulfillmentStatus status) => status switch
    {
        FulfillmentStatus.Pending => "Pendiente",
        FulfillmentStatus.Complete => "Surtido completo",
        FulfillmentStatus.Partial => "Surtido parcial",
        FulfillmentStatus.NotFulfilled => "No surtido",
        _ => status.ToString()
    };

    public static string ToneFor(FulfillmentStatus status) => status switch
    {
        FulfillmentStatus.Pending => "pending",
        FulfillmentStatus.Complete => "complete",
        FulfillmentStatus.Partial => "partial",
        FulfillmentStatus.NotFulfilled => "none",
        _ => "pending"
    };

    public static string For(UserRole role) => role switch
    {
        UserRole.Production => "Producción",
        UserRole.Warehouse => "Almacén",
        UserRole.Supervisor => "Supervisor",
        UserRole.Administrator => "Administrador",
        _ => role.ToString()
    };
}
