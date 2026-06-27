using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MetodosPagoController : Controller
    {
        private readonly AppDbContext _context;
        public MetodosPagoController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var metodos = await _context.MetodosPago
                .Include(m => m.Pedidos)
                    .ThenInclude(p => p.Clientes)
                .Include(m => m.Pedidos)
                    .ThenInclude(p => p.Estado)
                .ToListAsync();
            return View(metodos);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MetodoPago metodo)
        {
            if (!ModelState.IsValid) return View(metodo);
            _context.Add(metodo);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Método de pago creado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var m = await _context.MetodosPago.FindAsync(id);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MetodoPago metodo)
        {
            if (id != metodo.MetodoPagoId) return NotFound();
            if (!ModelState.IsValid) return View(metodo);
            _context.Update(metodo);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Método de pago actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _context.MetodosPago.FindAsync(id);
            if (m != null) { _context.MetodosPago.Remove(m); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Método de pago eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}