using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProveedoresController : Controller
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Proveedores
        public async Task<IActionResult> Index()
        {
            // Incluimos la categoría asignada para mostrarla en la tabla
            var lista = await _context.Proveedores
                .Include(p => p.Categoria)
                .ToListAsync();

            // Pasamos las prendas y categorías para el modal de pedidos masivos
            ViewBag.Prendas = await _context.Prendas.ToListAsync();
            return View(lista);
        }

        // GET: Proveedores/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "CategoriaId", "NombreCategoria");
            return View();
        }

        // POST: Proveedores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proveedor proveedor)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "CategoriaId", "NombreCategoria", proveedor.CategoriaId);
                return View(proveedor);
            }

            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Proveedor registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound();

            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "CategoriaId", "NombreCategoria", p.CategoriaId);
            return View(p);
        }

        // POST: Proveedores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.ProveedorId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "CategoriaId", "NombreCategoria", proveedor.CategoriaId);
                return View(proveedor);
            }

            _context.Update(proveedor);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Proveedor actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Proveedores/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p != null)
            {
                _context.Proveedores.Remove(p);
                await _context.SaveChangesAsync();
            }
            TempData["OK"] = "Proveedor eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // 🔥 NUEVA ACCIÓN: HACER PEDIDO (Abastecer Stock de prendas según la categoría del proveedor)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HacerPedido(int proveedorId, int cantidadPedida)
        {
            if (cantidadPedida <= 0)
            {
                TempData["ErrorVenta"] = "La cantidad del pedido debe ser mayor a cero.";
                return RedirectToAction(nameof(Index));
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.ProveedorId == proveedorId);

            if (proveedor == null || proveedor.CategoriaId == null)
            {
                TempData["ErrorVenta"] = "El proveedor no existe o no tiene una categoría asignada.";
                return RedirectToAction(nameof(Index));
            }

            // El campo correcto de tu modelo Prenda es CategoriaId (en singular) 
            var variantesAbastecer = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .Where(v => v.Prenda.CategoriaId == proveedor.CategoriaId)
                .ToListAsync();

            if (!variantesAbastecer.Any())
            {
                TempData["ErrorVenta"] = "No hay prendas registradas bajo la categoría de este proveedor para abastecer.";
                return RedirectToAction(nameof(Index));
            }

            // Incrementamos el stock uniformemente a cada variante del proveedor
            foreach (var variante in variantesAbastecer)
            {
                variante.Stock += cantidadPedida;
            }

            await _context.SaveChangesAsync();

            TempData["OK"] = $"¡Pedido realizado con éxito! Se añadieron {cantidadPedida} unidades a todas las variantes correspondientes.";
            return RedirectToAction(nameof(Index));
        }
    }
}