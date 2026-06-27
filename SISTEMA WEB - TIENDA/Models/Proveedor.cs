using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Proveedor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProveedorId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }

        [Required, StringLength(11)]
        [Display(Name = "RUC")]
        public string RUC { get; set; }

        [StringLength(100)]
        [Display(Name = "Contacto")]
        public string Contacto { get; set; }

        [StringLength(15)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}