using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public Usuario() : base() { }
    }
}