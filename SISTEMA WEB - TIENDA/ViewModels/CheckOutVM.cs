using System.ComponentModel.DataAnnotations;

namespace SISTEMA_WEB___TIENDA.ViewModels
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "La calle o avenida es obligatoria.")]
        [StringLength(200)]
        [Display(Name = "Calle / Avenida")]
        public string CalleAvenida { get; set; } = string.Empty;

        [Required(ErrorMessage = "El distrito es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Distrito")]
        public string Distrito { get; set; } = string.Empty;

        [Required(ErrorMessage = "La referencia es obligatoria.")]
        [StringLength(150)]
        [Display(Name = "Referencia")]
        public string Referencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un método de pago.")]
        [Display(Name = "Método de Pago")]
        public int MetodoPagoId { get; set; }
    }
}