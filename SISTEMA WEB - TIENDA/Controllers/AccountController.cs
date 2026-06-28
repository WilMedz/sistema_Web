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
        public async Task<IActionResult> Index(string? searchString) 
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

            
            var historialQuery = _context.Pedidos
                .Where(p => p.ClientesId == clienteId)
                .Include(p => p.MetodoPago)
                .Include(p => p.Estado)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(searchString))
            {
                historialQuery = historialQuery.Where(p => p.PedidoId.ToString().Contains(searchString));
            }

            
            var historial = await historialQuery
                .OrderByDescending(p => p.PedidoId)
                .ToListAsync();

            
            ViewBag.CurrentFilter = searchString;

            
            var model = new PerfilViewModel
            {
                NombreUsuario = cliente.Nombres,
                Email = cliente.CorreoElectronico,
                HistorialPedidos = historial,
                Carrito = HttpContext.Session.GetObject<List<CarritoItem>>("Carrito") ?? new List<CarritoItem>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DetallePedido(int id)
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null) return RedirectToAction("Login", "Login");

            var pedido = await _context.Pedidos
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.ClientesId == clienteId);

            if (pedido == null) return NotFound();

            return View(pedido);
        }
    }
}
