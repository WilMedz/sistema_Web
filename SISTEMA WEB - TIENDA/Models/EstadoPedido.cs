using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class EstadoPedido
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EstadoId { get; set; }

        [Required, StringLength(30)]
        public string NombreEstado { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
