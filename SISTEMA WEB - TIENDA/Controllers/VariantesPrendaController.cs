using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VariantesPrendaController : Controller
    {
        private readonly AppDbContext _context;
        public VariantesPrendaController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var variantes = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .OrderBy(v => v.Prenda.NombrePrenda)
                .ToListAsync();
            return View(variantes);
        }

        public IActionResult Create()
        {
            ViewBag.Prendas = new SelectList(_context.Prendas, "PrendaId", "NombrePrenda");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VariantePrenda variante)
        {
            ModelState.Remove("SKU");
            ModelState.Remove("Prenda");
            ModelState.Remove("DetallesPedido");

            if (ModelState.IsValid)
            {
                var prenda = await _context.Prendas.FindAsync(variante.PrendaId);
                if (prenda == null)
                {
                    ModelState.AddModelError("PrendaId", "La prenda seleccionada no existe.");
                }
                else
                {
                    variante.SKU = await GenerarSkuUnicoAsync(prenda, variante.Talla, variante.Color);
                    _context.Add(variante);
                    await _context.SaveChangesAsync();
                    TempData["OK"] = "Variante registrada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Prendas = new SelectList(_context.Prendas, "PrendaId", "NombrePrenda", variante.PrendaId);
            return View(variante);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VariantePrenda variante)
        {
            ModelState.Remove("Prenda");
            ModelState.Remove("DetallesPedido");

            if (id != variante.VarianteId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(variante);
                await _context.SaveChangesAsync();
                TempData["OK"] = "Variante actualizada.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Prendas = new SelectList(_context.Prendas, "PrendaId", "NombrePrenda", variante.PrendaId);
            return View(variante);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var v = await _context.VariantesPrenda.FindAsync(id);
            if (v == null) return NotFound();
            ViewBag.Prendas = new SelectList(_context.Prendas, "PrendaId", "NombrePrenda", v.PrendaId);
            return View(v);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _context.VariantesPrenda.FindAsync(id);
            if (v != null) { _context.VariantesPrenda.Remove(v); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Variante eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // Se genera un SKU (codigo unico de prenda ) y verifica que no exista en la bd, para que no se repita 
        private async Task<string> GenerarSkuUnicoAsync(Prenda prenda, string talla, string color)
        {
            string basePrenda = prenda.NombrePrenda
                .Replace(" ", "")
                .Substring(0, Math.Min(3, prenda.NombrePrenda.Replace(" ", "").Length))
                .ToUpper();

            string colorLimpio = color
                .Replace(" ", "")
                .Substring(0, Math.Min(3, color.Replace(" ", "").Length))
                .ToUpper();

            string tallaLimpia = talla.Replace(" ", "").ToUpper();

            string skuBase = $"{basePrenda}{prenda.PrendaId}-{tallaLimpia}-{colorLimpio}";

            string sku = skuBase;
            int contador = 1;
            while (await _context.VariantesPrenda.AnyAsync(v => v.SKU == sku))
            {
                contador++;
                sku = $"{skuBase}-{contador}";
            }

            return sku;
        }
    }
}