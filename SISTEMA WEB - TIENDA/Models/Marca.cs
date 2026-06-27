using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Marca
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MarcaId { get; set; }

        [Required, StringLength(50)]
        public string NombreMarca { get; set; }

        public virtual ICollection<Prenda> Prendas { get; set; } = new List<Prenda>();
    }
}
