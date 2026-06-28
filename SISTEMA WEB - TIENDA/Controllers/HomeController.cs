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

    // 1. VER CATÁLOGO
    public async Task<IActionResult> Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Login");
        }

        var catálogo = await _context.Prendas
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .Include(p => p.Variantes) // Carga las variantes disponibles (Tallas/Colores/Stock)
            .ToListAsync();

        return View(catálogo);
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

    // AGREGAR AL CARRITO (Adaptado al nuevo CarritoItem)
    [HttpPost]
    public async Task<IActionResult> AgregarAlCarrito(int prendaId, int varianteId, int cantidad, string talla, string color)
    {
        var prenda = await _context.Prendas.FindAsync(prendaId);
        if (prenda == null) return NotFound();

        var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionCarrito) ?? new List<CarritoItem>();

        // Ahora buscamos si ya existe en el carrito la variante específica (mismo ID de variante)
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

    // 3. HACER UN PEDIDO (Finalizar Compra asociando las variantes)
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

        // Recuperar ID del cliente (Asegúrate de tenerlo mapeado en la sesión en tu Login)
        int clienteId = HttpContext.Session.GetInt32("UsuarioId") ?? 3; // Por defecto Cliente Prueba (ID: 3)

        // Crear la entidad principal del Pedido
        var nuevoPedido = new Pedido
        {
            FechaPedido = DateTime.Now,
            ClientesId = clienteId,
            DireccionId = 1,     // Dirección por defecto (Jr. Principal 123)
            MetodoPagoId = 1,    // Yape / Plin por defecto
            EstadoId = 1,        // Estado: Pendiente
            TotalCompra = carrito.Sum(item => item.Total),
            Detalles = new List<DetallePedido>()
        };

        // Mapeamos cada variante del carrito hacia los detalles de la compra
        foreach (var item in carrito)
        {
            var detalle = new DetallePedido
            {
                PrecioUnitario = item.Precio,
                Cantidad = item.Cantidad
                // NOTA: Si tu entidad DetallePedido tiene la propiedad 'VariantePrendaId' o 'VarianteId', 
                // asígnala aquí abajo descomentando la línea que corresponda a tu modelo:
                // VariantePrendaId = item.VarianteId 
            };
            nuevoPedido.Detalles.Add(detalle);
        }

        _context.Pedidos.Add(nuevoPedido);
        await _context.SaveChangesAsync();

        // Limpiar el carrito de la sesión tras registrar la compra con éxito
        HttpContext.Session.Remove(SessionCarrito);

        // 4. REDIRIGIR INMEDIATAMENTE AL COMPROBANTE EN PANTALLA
        return RedirectToAction("Comprobante", new { id = nuevoPedido.PedidoId });
    }

    // 4. GENERACIÓN DE SU COMPROBANTE
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