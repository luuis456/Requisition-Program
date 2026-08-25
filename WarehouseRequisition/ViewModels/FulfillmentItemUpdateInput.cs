using System.ComponentModel.DataAnnotations;

namespace WarehouseRequisition.ViewModels;

/// <summary>Server-side input for the AJAX fulfillment updates.</summary>
public class FulfillmentItemUpdateInput
{
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int RequisitionId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int ItemId { get; set; }

    [Range(0, 999999, ErrorMessage = "La cantidad debe ser mayor o igual a cero.")]
    public decimal FulfilledQuantity { get; set; }

    public int? ShortageReasonId { get; set; }

    [StringLength(300, ErrorMessage = "El comentario no puede exceder 300 caracteres.")]
    public string? ShortageComment { get; set; }

    public bool Reviewed { get; set; }
}
