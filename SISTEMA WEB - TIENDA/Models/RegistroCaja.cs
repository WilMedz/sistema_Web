using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class RegistroCaja
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegistroId { get; set; }

        [Required]
        public int ClientesId { get; set; }
        public virtual Clientes Cajero { get; set; } = null!;

        [Required]
        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoInicial { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MontoFinal { get; set; }

        public bool EstaAbierta => FechaCierre == null;
    }
}
