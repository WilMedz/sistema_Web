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
                .Include(p => p.Cajero)
                .AsQueryable();

            if (User.IsInRole("Cajero"))
            {
                var cajeroId = HttpContext.Session.GetInt32("ClienteId");
                if (cajeroId.HasValue)
                    pedidos = pedidos.Where(p => p.CajeroId == cajeroId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
                pedidos = pedidos.Where(p => p.PedidoId.ToString().Contains(searchString));

            ViewBag.CurrentFilter = searchString;
            var lista = await pedidos.OrderByDescending(p => p.FechaPedido).ToListAsync();

            // Vista diferente según rol
            if (User.IsInRole("Administrador"))
                return View("IndexAdmin", lista);

            return View("IndexCajero", lista);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Include(p => p.Cajero)      // ← AGREGAR
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpGet]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p => p.PedidoId == id);
            if (pedido == null) return NotFound();

            ViewBag.Estados = new SelectList(
                await _context.EstadosPedido.ToListAsync(),
                "EstadoId", "NombreEstado", pedido.EstadoId);

            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, int EstadoId)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.EstadoId = EstadoId;
            await _context.SaveChangesAsync();
            TempData["OK"] = "Estado del pedido actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();

            foreach (var detalle in pedido.Detalles)
            {
                var variante = await _context.VariantesPrenda.FindAsync(detalle.VarianteId);
                if (variante != null) variante.Stock += detalle.Cantidad;
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            TempData["OK"] = $"Pedido #{id:D6} eliminado y stock restaurado.";
            return RedirectToAction(nameof(Index));
        }
    }
}