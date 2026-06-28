using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador,Cajero")]
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;
        public PedidosController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? searchString)
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .AsQueryable(); 

            // 
            if (!string.IsNullOrEmpty(searchString))
            {
               
                pedidos = pedidos.Where(p => p.PedidoId.ToString().Contains(searchString));
            }

            var resultado = await pedidos
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            
            ViewBag.CurrentFilter = searchString;

            return View(resultado);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            // CORRECCIÓN: Se cambió "DescripcionEstado" por "NombreEstado" según la instrucción de tu vista
            ViewBag.Estados = new SelectList(
                await _context.EstadosPedido.ToListAsync(),
                "EstadoId", "NombreEstado", pedido.EstadoId);

            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id, int EstadoId)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.EstadoId = EstadoId;
            await _context.SaveChangesAsync();
            TempData["OK"] = "Estado del pedido actualizado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
