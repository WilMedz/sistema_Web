using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;
        public CategoriasController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Categorias.ToListAsync());

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Categoría creada.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId) return NotFound();
            if (!ModelState.IsValid) return View(categoria);
            _context.Update(categoria);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Categoría actualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c != null) { _context.Categorias.Remove(c); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Categoría eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}