using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class DetallePedido
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetalleId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "99999.99", ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        [Required]
        public int VarianteId { get; set; }
        public virtual VariantePrenda Variante { get; set; }
    }
}
