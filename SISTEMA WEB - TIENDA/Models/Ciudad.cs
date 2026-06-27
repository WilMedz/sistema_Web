using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISTEMA_WEB___TIENDA.Models
{
    public class Ciudad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CiudadId { get; set; }

        [Required, StringLength(50)]
        public string NombreCiudad { get; set; }

        public virtual ICollection<Clientes> Clientes { get; set; }
    }
}
