using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PrendasController : Controller
    {
        private readonly AppDbContext _context;
        public PrendasController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // Alerta stock crítico
            var stockCritico = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .Where(v => v.Stock <= 3)
                .ToListAsync();
            ViewBag.StockCritico = stockCritico;

            var prendas = await _context.Prendas
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .ToListAsync();
            return View(prendas);
        }

        public IActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(_context.Categorias, "CategoriaId", "NombreCategoria");
            ViewBag.MarcaId = new SelectList(_context.Marcas, "MarcaId", "NombreMarca");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prenda prenda)
        {
            // Ignorar propiedades de navegación en la validación
            ModelState.Remove("Categoria");
            ModelState.Remove("Marca");
            ModelState.Remove("Variantes");

            if (ModelState.IsValid)
            {
                _context.Add(prenda);
                await _context.SaveChangesAsync();
                TempData["OK"] = "Prenda creada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Ver qué campos fallan (temporal para debug)
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Any())
                    Console.WriteLine($"Campo: {error.Key} → {error.Value.Errors.First().ErrorMessage}");
            }

            ViewBag.CategoriaId = new SelectList(_context.Categorias, "CategoriaId", "NombreCategoria", prenda.CategoriaId);
            ViewBag.MarcaId = new SelectList(_context.Marcas, "MarcaId", "NombreMarca", prenda.MarcaId);
            return View(prenda);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Prendas.FindAsync(id);
            if (p == null) return NotFound();
            ViewBag.CategoriaId = new SelectList(_context.Categorias, "CategoriaId", "NombreCategoria", p.CategoriaId);
            ViewBag.MarcaId = new SelectList(_context.Marcas, "MarcaId", "NombreMarca", p.MarcaId);
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prenda prenda)
        {
            ModelState.Remove("Categoria");
            ModelState.Remove("Marca");
            ModelState.Remove("Variantes");

            if (id != prenda.PrendaId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(prenda);
                await _context.SaveChangesAsync();
                TempData["OK"] = "Prenda actualizada.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoriaId = new SelectList(_context.Categorias, "CategoriaId", "NombreCategoria", prenda.CategoriaId);
            ViewBag.MarcaId = new SelectList(_context.Marcas, "MarcaId", "NombreMarca", prenda.MarcaId);
            return View(prenda);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Prendas.FindAsync(id);
            if (p != null) { _context.Prendas.Remove(p); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Prenda eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}