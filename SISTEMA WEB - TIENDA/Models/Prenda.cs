using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Prenda
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrendaId { get; set; }

        [Required, StringLength(150)]
        public string NombrePrenda { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioLista { get; set; }

        [Range(0, 99999.99, ErrorMessage = "El precio de oferta no puede ser negativo.")]
        [OfertaPrecioMenor("PrecioLista", ErrorMessage = "El precio de oferta debe ser menor al precio de lista.")]
        public decimal? PrecioOferta { get; set; }

        [Required, StringLength(255)]
        public string ImagenPrincipalUrl { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        [Required]
        public int MarcaId { get; set; }
        public virtual Marca Marca { get; set; }

        public virtual ICollection<VariantePrenda> Variantes { get; set; }
    }
}