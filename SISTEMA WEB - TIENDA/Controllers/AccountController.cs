using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Extensions;
using SISTEMA_WEB___TIENDA.Models;
using SISTEMA_WEB___TIENDA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var historial = await _context.Pedidos
                .Where(p => p.ClientesId == clienteId)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .OrderByDescending(p => p.PedidoId)
                .ToListAsync();

            var model = new PerfilViewModel
            {
                NombreUsuario = cliente.Nombres,
                Email = cliente.CorreoElectronico,
                HistorialPedidos = historial,
                Carrito = HttpContext.Session.GetObject<List<CarritoItem>>("Carrito") ?? new List<CarritoItem>()
            };

            return View(model);
        }
    }
}
