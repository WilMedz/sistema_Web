using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProveedoresController : Controller
    {
        private readonly AppDbContext _context;
        public ProveedoresController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var proveedores = await _context.Proveedores.ToListAsync();
            return View(proveedores);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proveedor proveedor)
        {
            if (!ModelState.IsValid) return View(proveedor);
            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Proveedor registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.ProveedorId) return NotFound();
            if (!ModelState.IsValid) return View(proveedor);
            _context.Update(proveedor);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Proveedor actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p != null) { _context.Proveedores.Remove(p); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Proveedor eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}