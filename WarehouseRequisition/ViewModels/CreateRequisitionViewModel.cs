using System.ComponentModel.DataAnnotations;

namespace WarehouseRequisition.ViewModels;

public class RequisitionItemInputViewModel
{
    [Required(ErrorMessage = "El número de parte es obligatorio.")]
    [StringLength(50, ErrorMessage = "El número de parte no puede exceder 50 caracteres.")]
    public string PartNumber { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres.")]
    public string? Description { get; set; }

    [Range(0.01, 999999, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public decimal RequestedQuantity { get; set; }

    [StringLength(100, ErrorMessage = "La descripción de cantidad no puede exceder 100 caracteres.")]
    public string? QuantityDescription { get; set; }

    [StringLength(20)]
    public string UnitOfMeasure { get; set; } = "PZA";

    [StringLength(30)]
    public string? Location { get; set; }

    [StringLength(300, ErrorMessage = "Las observaciones no pueden exceder 300 caracteres.")]
    public string? Observations { get; set; }
}

public class CreateRequisitionViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime RequestedDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    [Display(Name = "Nombre del requisitor")]
    public string RequesterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [StringLength(20, ErrorMessage = "El número de reloj no puede exceder 20 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de reloj solo puede contener dígitos.")]
    [Display(Name = "Número de reloj")]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar una planta.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una planta.")]
    public int PlantId { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un área.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un área.")]
    public int AreaId { get; set; }

    [Required(ErrorMessage = "Debes seleccionar una máquina.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una máquina.")]
    public int MachineId { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres.")]
    public string? Notes { get; set; }

    public List<RequisitionItemInputViewModel> Items { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Items.Count == 0)
        {
            yield return new ValidationResult(
                "Debes agregar al menos un material.",
                [nameof(Items)]);
        }
    }
}
