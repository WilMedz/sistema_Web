using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;
using SISTEMA_WEB___TIENDA.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SISTEMA_WEB___TIENDA.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction("Admin", "Login");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var usuario = await _context.Clientes
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.CorreoElectronico == modelo.Correo);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo o la contraseña son incorrectos.");
                    return View(modelo);
                }


                if (!BCrypt.Net.BCrypt.Verify(modelo.Password, usuario.Contrasena))
                {
                    ModelState.AddModelError(string.Empty, "El correo o la contraseña son incorrectos.");
                    return View(modelo);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombres),
                    new Claim(ClaimTypes.Email, usuario.CorreoElectronico),
                    new Claim(ClaimTypes.Role, usuario.Rol.NombreRol),
                    new Claim("UsuarioId", usuario.ClienteId.ToString())
                };

              

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.SetInt32("ClienteId", usuario.ClienteId);
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombres);
                HttpContext.Session.SetString("UsuarioRol", usuario.Rol.NombreRol);

                if (usuario.Rol.NombreRol == "Administrador")
                {
                    return RedirectToAction("Admin", "Login");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                ModelState.AddModelError(string.Empty, " El esquema de la base de datos no coincide. ");
                return View(modelo);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error al procesar su solicitud. ");
                return View(modelo);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            var listaCiudades = await _context.Ciudades.ToListAsync();
            ViewBag.Ciudades = new SelectList(listaCiudades, "CiudadId", "NombreCiudad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroVM modelo)
        {
            if (!ModelState.IsValid)
            {
                var listaCiudades = await _context.Ciudades.ToListAsync();
                ViewBag.Ciudades = new SelectList(listaCiudades, "CiudadId", "NombreCiudad");
                return View(modelo);
            }

            var existeCorreo = await _context.Clientes.AnyAsync(c => c.CorreoElectronico == modelo.CorreoElectronico);
            if (existeCorreo)
            {
                ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                var listaCiudades = await _context.Ciudades.ToListAsync();
                ViewBag.Ciudades = new SelectList(listaCiudades, "CiudadId", "NombreCiudad");
                return View(modelo);
            }

            var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Cliente");
            int idRolDefault = rolCliente != null ? rolCliente.RolId : 2;

            var nuevoCliente = new Clientes
            {
                Nombres = modelo.Nombres,
                CorreoElectronico = modelo.CorreoElectronico,
                CiudadId = modelo.CuidadId,
                RolId = idRolDefault,
                FechaNacimiento = modelo.FechaNacimiento
            };


            nuevoCliente.Contrasena = BCrypt.Net.BCrypt.HashPassword(modelo.Contrasena);

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            TempData["MensajeRegistro"] = "¡Registro exitoso!";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var hoy = DateTime.Now;

            ViewBag.TotalPrendas = await _context.Prendas.CountAsync();
            ViewBag.TotalClientes = await _context.Clientes.CountAsync(c => c.RolId == 2);
            ViewBag.PedidosHoy = await _context.Pedidos
                                        .CountAsync(p => p.FechaPedido.Date == hoy.Date);
            ViewBag.VentasMes = await _context.Pedidos
                                        .Where(p => p.FechaPedido.Year == hoy.Year && p.FechaPedido.Month == hoy.Month)
                                        .SumAsync(p => (decimal?)p.TotalCompra) ?? 0;
            ViewBag.StockCritico = await _context.VariantesPrenda
                                        .Include(v => v.Prenda)
                                        .Where(v => v.Stock <= 3)
                                        .ToListAsync();

            return View();
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
