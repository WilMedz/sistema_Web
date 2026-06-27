using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Clientes
                .Include(c => c.Rol)
                .Include(c => c.Ciudad)
                .Where(c => c.RolId != 2) // solo para administrador y cajero
                .ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _context.Roles.Where(r => r.RolId != 2).ToListAsync(), "RolId", "NombreRol");
            ViewBag.Ciudades = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
    
        public async Task<IActionResult> Create(Clientes cliente, string Contrasena)
        {
            ModelState.Remove("Contrasena");
            ModelState.Remove("Rol");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Direcciones");
            ModelState.Remove("Pedidos");

            if (await _context.Clientes.AnyAsync(c => c.CorreoElectronico == cliente.CorreoElectronico))
                ModelState.AddModelError("CorreoElectronico", "Este correo ya está registrado.");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _context.Roles.Where(r => r.RolId != 2).ToListAsync(), "RolId", "NombreRol");
                ViewBag.Ciudades = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad");
                return View(cliente);
            }

            cliente.Contrasena = new PasswordHasher<Clientes>().HashPassword(cliente, Contrasena);
            _context.Add(cliente);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return NotFound();
            ViewBag.Roles = new SelectList(await _context.Roles.Where(r => r.RolId != 2).ToListAsync(), "RolId", "NombreRol", c.RolId);
            ViewBag.Ciudades = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad", c.CiudadId);
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Clientes cliente, string? NuevaContrasena)
        {
            ModelState.Remove("Contrasena");
            ModelState.Remove("Rol");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Direcciones");
            ModelState.Remove("Pedidos");


            if (id != cliente.ClienteId) return NotFound();

            var original = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.ClienteId == id);
            if (original == null) return NotFound();

            cliente.Contrasena = string.IsNullOrEmpty(NuevaContrasena)
                ? original.Contrasena
                : BCrypt.Net.BCrypt.HashPassword(NuevaContrasena);

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _context.Roles.Where(r => r.RolId != 2).ToListAsync(), "RolId", "NombreRol");
                ViewBag.Ciudades = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad");
                return View(cliente);
            }

            _context.Update(cliente);
            await _context.SaveChangesAsync();
            TempData["OK"] = "Usuario actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c != null) { _context.Clientes.Remove(c); await _context.SaveChangesAsync(); }
            TempData["OK"] = "Usuario eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}