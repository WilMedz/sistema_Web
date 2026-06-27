using System.Collections.Generic;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.ViewModels
{
    public class PerfilViewModel
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public List<Pedido> HistorialPedidos { get; set; }
        public List<CarritoItem> Carrito { get; set; }
    }
}
