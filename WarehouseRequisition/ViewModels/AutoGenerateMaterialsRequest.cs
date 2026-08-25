using System.ComponentModel.DataAnnotations;

namespace WarehouseRequisition.ViewModels;

/// <summary>
/// Request for the automatic material generation endpoint.
/// Today it is served by a mock generator; later it will be replaced by a real
/// integration against the BOM / ERP / SAP / AS400 production system.
/// </summary>
public class AutoGenerateMaterialsRequest
{
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [StringLength(30)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [StringLength(20)]
    public string Line { get; set; } = string.Empty;

    [Range(1, 15, ErrorMessage = "La cantidad debe estar entre 1 y 15.")]
    public int Quantity { get; set; } = 3;
}
