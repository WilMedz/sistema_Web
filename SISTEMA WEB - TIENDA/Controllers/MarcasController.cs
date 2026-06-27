using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MarcasController : Controller
    {
        private readonly AppDbContext _context;
        public MarcasController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var marcas = await _context.Marcas
                .Include(m => m.Prendas)
                .ToListAsync();
            return View(marcas);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            if (!ModelState.IsValid) return View(marca);
            _context.Add(marca);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Marca creada.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var m = await _context.Marcas.FindAsync(id);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            if (id != marca.MarcaId) return NotFound();
            if (!ModelState.IsValid) return View(marca);
            _context.Update(marca);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Marca actualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _context.Marcas.FindAsync(id);
            if (m != null) { _context.Marcas.Remove(m); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Marca eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}