using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class VariantePrenda
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VarianteId { get; set; }

        [Required, StringLength(50)]
        public string SKU { get; set; }

        [Required, StringLength(20)]
        public string Talla { get; set; }

        [Required, StringLength(30)]
        public string Color { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int PrendaId { get; set; }
        public virtual Prenda Prenda { get; set; }

        public virtual ICollection<DetallePedido> DetallesPedido { get; set; }
    }
}
