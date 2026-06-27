using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Clientes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClienteId { get; set; }

        [Required, StringLength(100)]
        public string Nombres { get; set; }

        [Required, StringLength(100)]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        public string CorreoElectronico { get; set; }

        [Required, StringLength(255)]
        public string Contrasena { get; set; }

        [Required]
        public int CiudadId { get; set; }
        public virtual Ciudad Ciudad { get; set; }

        [Required]
        public int RolId { get; set; }
        public virtual Roles Rol { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MayorDeEdad]
        public DateTime FechaNacimiento { get; set; }

        public virtual ICollection<DireccionEnvio> Direcciones { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
