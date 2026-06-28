using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;
using SISTEMA_WEB___TIENDA.Extensions;

namespace SISTEMA_WEB___TIENDA.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private const string SessionCarrito = "CarritoAleana";

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // 1. VER CATÁLOGO (CON FILTRO DE BÚSQUEDA)
    public async Task<IActionResult> Index(string buscar)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var prendasQuery = _context.Prendas
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
            .AsQueryable();

        if (!string.IsNullOrEmpty(buscar))
        {
            buscar = buscar.Trim().ToLower();
            prendasQuery = prendasQuery.Where(p => p.NombrePrenda.ToLower().Contains(buscar)
                                                || p.Marca.NombreMarca.ToLower().Contains(buscar)
                                                || p.Categoria.NombreCategoria.ToLower().Contains(buscar));
        }

        var catálogo = await prendasQuery.ToListAsync();
        return View(catálogo);
    }

    // NEW: ENDPOINT PARA SUGERENCIAS EN TIEMPO REAL (AJAX)
    [HttpGet]
    public async Task<JsonResult> BuscarSugerencias(string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return Json(new List<object>());
        }

        term = term.Trim().ToLower();

        // Buscamos las prendas que coincidan en nombre, marca o categoría, limitando a un top 6 para optimizar velocidad
        var sugerencias = await _context.Prendas
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .Where(p => p.NombrePrenda.ToLower().Contains(term)
                     || p.Marca.NombreMarca.ToLower().Contains(term)
                     || p.Categoria.NombreCategoria.ToLower().Contains(term))
            .Take(6)
            .Select(p => new
            {
                id = p.PrendaId,
                nombre = p.NombrePrenda,
                precio = p.PrecioOferta ?? p.PrecioLista,
                imagen = p.ImagenPrincipalUrl
            })
            .ToListAsync();

        return Json(sugerencias);
    }

    // 2. VER CARRITO
    public IActionResult Carrito()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionCarrito) ?? new List<CarritoItem>();
        return View(carrito);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarAlCarrito(int prendaId, int varianteId, int cantidad, string talla, string color)
    {
        var prenda = await _context.Prendas.FindAsync(prendaId);
        if (prenda == null) return NotFound();

        var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionCarrito) ?? new List<CarritoItem>();

        var itemExistente = carrito.FirstOrDefault(c => c.VarianteId == varianteId);
        if (itemExistente != null)
        {
            itemExistente.Cantidad += cantidad;
        }
        else
        {
            carrito.Add(new CarritoItem
            {
                VarianteId = varianteId,
                PrendaId = prenda.PrendaId,
                NombrePrenda = prenda.NombrePrenda,
                Talla = talla,
                Color = color,
                Precio = prenda.PrecioOferta ?? prenda.PrecioLista,
                Cantidad = cantidad,
                ImagenUrl = prenda.ImagenPrincipalUrl
            });
        }

        HttpContext.Session.SetObject(SessionCarrito, carrito);
        return RedirectToAction("Index");
    }

    // 3. HACER UN PEDIDO (Finalizar Compra)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FinalizarCompra()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionCarrito) ?? new List<CarritoItem>();
        if (!carrito.Any())
        {
            TempData["ErrorCarrito"] = "Tu bolsa de compras está vacía.";
            return RedirectToAction("Carrito");
        }

        int clienteId = HttpContext.Session.GetInt32("UsuarioId") ?? 3;

        var nuevoPedido = new Pedido
        {
            FechaPedido = DateTime.Now,
            ClientesId = clienteId,
            DireccionId = 1,
            MetodoPagoId = 1,
            EstadoId = 1,
            TotalCompra = carrito.Sum(item => item.Total),
            Detalles = new List<DetallePedido>()
        };

        foreach (var item in carrito)
        {
            var detalle = new DetallePedido
            {
                PrecioUnitario = item.Precio,
                Cantidad = item.Cantidad
            };
            nuevoPedido.Detalles.Add(detalle);
        }

        _context.Pedidos.Add(nuevoPedido);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove(SessionCarrito);
        return RedirectToAction("Comprobante", new { id = nuevoPedido.PedidoId });
    }

    // 4. GENERACIÓN DE COMPROBANTE
    public async Task<IActionResult> Comprobante(int id)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var pedido = await _context.Pedidos
            .Include(p => p.Clientes)
            .Include(p => p.DireccionEnvio)
            .Include(p => p.MetodoPago)
            .Include(p => p.Estado)
            .FirstOrDefaultAsync(p => p.PedidoId == id);
        if (pedido == null) return NotFound();

        return View(pedido);
    }

    // 5. VER ESTADO DE PEDIDO (Rastreo)
    public IActionResult MisPedidos()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuscarEstado(int pedidoId)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var pedido = await _context.Pedidos
            .Include(p => p.Estado)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        if (pedido == null)
        {
            TempData["ErrorRastreo"] = "No se encontró ningún pedido con el número ingresado.";
            return RedirectToAction("MisPedidos");
        }

        ViewBag.IdBuscado = pedido.PedidoId;
        ViewBag.Estado = pedido.Estado?.NombreEstado ?? "Pendiente";
        ViewBag.Fecha = pedido.FechaPedido.ToString("dd/MM/yyyy");
        ViewBag.Total = pedido.TotalCompra.ToString("F2");

        return View("MisPedidos");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}