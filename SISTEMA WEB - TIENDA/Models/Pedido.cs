using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Pedido
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PedidoId { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "El total de la compra debe ser mayor a 0.")]
        public decimal TotalCompra { get; set; }

        [Required]
        public int ClientesId { get; set; }
        public virtual Clientes Clientes { get; set; }

        [Required]
        public int DireccionId { get; set; }
        public virtual DireccionEnvio DireccionEnvio { get; set; }

        [Required]
        public int MetodoPagoId { get; set; }
        public virtual MetodoPago MetodoPago { get; set; }

        [Required]
        public int EstadoId { get; set; }
        public virtual EstadoPedido Estado { get; set; }

        // Cajero responsable (null = pedido hecho por web)
        public int? CajeroId { get; set; }
        public virtual Clientes? Cajero { get; set; }

        public virtual ICollection<DetallePedido> Detalles { get; set; }
    }
}