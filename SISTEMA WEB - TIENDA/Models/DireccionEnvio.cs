using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class DireccionEnvio
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DireccionId { get; set; }

        [Required, StringLength(200)]
        public string CalleAvenida { get; set; }

        [Required, StringLength(100)]
        public string Distrito { get; set; }

        [Required, StringLength(150)]
        public string Referencia { get; set; }

        [Required]
        public int ClientesId { get; set; }
        public virtual Clientes Clientes { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
