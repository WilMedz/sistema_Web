using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Categoria
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoriaId { get; set; }

        [Required, StringLength(50)]
        public string NombreCategoria { get; set; }

        public virtual ICollection<Prenda> Prendas { get; set; } = new List<Prenda>();

    }
}
