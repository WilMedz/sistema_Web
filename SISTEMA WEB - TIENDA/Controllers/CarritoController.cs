using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Extensions;
using SISTEMA_WEB___TIENDA.Models;
using SISTEMA_WEB___TIENDA.ViewModels;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;
        private const string SessionKey = "CarritoAleana";

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            ViewBag.GranTotal = carrito.Sum(i => i.Total);
            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(int prendaId, int cantidad = 1)
        {
            var prenda = await _context.Prendas.FindAsync(prendaId);
            if (prenda == null) return NotFound();

            // 🔍 VALIDACIÓN DE STOCK: Buscar la disponibilidad en las variantes de esta prenda
            var totalStockDisponible = await _context.VariantesPrenda
                .Where(v => v.PrendaId == prendaId)
                .SumAsync(v => v.Stock);

            if (totalStockDisponible <= 0)
            {
                TempData["ErrorStock"] = "Lo sentimos, este producto se encuentra temporalmente agotado.";
                return RedirectToAction("Index", "Home"); // O a la vista de detalles de la prenda
            }

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();

            var itemExistente = carrito.FirstOrDefault(i => i.PrendaId == prendaId);
            int cantidadTotalSolicitada = cantidad + (itemExistente?.Cantidad ?? 0);

            // Validar que lo acumulado no supere el stock físico real
            if (cantidadTotalSolicitada > totalStockDisponible)
            {
                TempData["ErrorStock"] = $"No puedes agregar esa cantidad. Stock máximo disponible: {totalStockDisponible}.";
                return RedirectToAction("Index");
            }

            decimal precio = (prenda.PrecioOferta.HasValue && prenda.PrecioOferta > 0)
                ? prenda.PrecioOferta.Value
                : prenda.PrecioLista;

            if (itemExistente != null)
                itemExistente.Cantidad += cantidad;
            else
                carrito.Add(new CarritoItem
                {
                    PrendaId = prenda.PrendaId,
                    NombrePrenda = prenda.NombrePrenda,
                    ImagenUrl = prenda.ImagenPrincipalUrl,
                    Precio = precio,
                    Cantidad = cantidad
                });

            HttpContext.Session.SetObject(SessionKey, carrito);
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int prendaId)
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            var item = carrito.FirstOrDefault(i => i.PrendaId == prendaId);
            if (item != null) carrito.Remove(item);
            HttpContext.Session.SetObject(SessionKey, carrito);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var usuarioId = HttpContext.Session.GetInt32("ClienteId");
            if (usuarioId == null) return RedirectToAction("Login", "Login");

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            if (!carrito.Any()) return RedirectToAction("Index");

            ViewBag.GranTotal = carrito.Sum(i => i.Total);
            ViewBag.TotalItems = carrito.Sum(i => i.Cantidad);
            ViewBag.MetodosPago = new SelectList(
                await _context.MetodosPago.ToListAsync(),
                "MetodoPagoId", "DescripcionPago");

            return View(new CheckoutVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM modelo)
        {
            var usuarioId = HttpContext.Session.GetInt32("ClienteId");
            if (usuarioId == null) return RedirectToAction("Login", "Login");

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();

            if (!carrito.Any()) return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                ViewBag.GranTotal = carrito.Sum(i => i.Total);
                ViewBag.TotalItems = carrito.Sum(i => i.Cantidad);
                ViewBag.MetodosPago = new SelectList(
                    await _context.MetodosPago.ToListAsync(),
                    "MetodoPagoId", "DescripcionPago");
                return View(modelo);
            }

            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Guardar dirección de envío
                var direccion = new DireccionEnvio
                {
                    CalleAvenida = modelo.CalleAvenida,
                    Distrito = modelo.Distrito,
                    Referencia = modelo.Referencia,
                    ClientesId = usuarioId.Value
                };
                _context.DireccionesEnvio.Add(direccion);
                await _context.SaveChangesAsync();

                // 2. Buscar estado "Pendiente"
                var estado = await _context.EstadosPedido
                    .FirstOrDefaultAsync(e => e.NombreEstado == "Pendiente")
                    ?? await _context.EstadosPedido.FirstAsync();

                // 3. Crear pedido
                var pedido = new Pedido
                {
                    FechaPedido = DateTime.Now,
                    TotalCompra = carrito.Sum(i => i.Total),
                    ClientesId = usuarioId.Value,
                    DireccionId = direccion.DireccionId,
                    MetodoPagoId = modelo.MetodoPagoId,
                    EstadoId = estado.EstadoId
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // 4. Crear detalles y descontar stock
                foreach (var item in carrito)
                {
                    // Buscamos la variante asignada de la prenda
                    var variante = await _context.VariantesPrenda
                        .FirstOrDefaultAsync(v => v.PrendaId == item.PrendaId);

                    if (variante == null)
                        throw new Exception($"Sin variante para producto '{item.NombrePrenda}' (ID {item.PrendaId}). Agrega una variante primero.");

                    // 🔍 VALIDACIÓN DE STOCK: Comprobar que hay suficiente cantidad física para la venta
                    if (item.Cantidad > variante.Stock)
                    {
                        throw new Exception($"Stock insuficiente para '{item.NombrePrenda}'. Solicitado: {item.Cantidad}, Disponible: {variante.Stock}.");
                    }

                    // 📉 RESTA DE STOCK: Modificación directa del inventario
                    variante.Stock -= item.Cantidad;
                    _context.VariantesPrenda.Update(variante);

                    _context.DetallesPedido.Add(new DetallePedido
                    {
                        PedidoId = pedido.PedidoId,
                        VarianteId = variante.VarianteId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio
                    });
                }

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                HttpContext.Session.Remove(SessionKey);
                TempData["NumeroPedido"] = pedido.PedidoId;
                TempData["DireccionEnvio"] = $"{modelo.CalleAvenida}, {modelo.Distrito}";
                TempData["MetodoPago"] = (await _context.MetodosPago
                    .FindAsync(modelo.MetodoPagoId))?.DescripcionPago;

                return RedirectToAction("OrdenConfirmada");
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                ModelState.AddModelError("", "Error al procesar: " + ex.Message);

                ViewBag.GranTotal = carrito.Sum(i => i.Total);
                ViewBag.TotalItems = carrito.Sum(i => i.Cantidad);
                ViewBag.MetodosPago = new SelectList(
                    await _context.MetodosPago.ToListAsync(),
                    "MetodoPagoId", "DescripcionPago");
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult OrdenConfirmada()
        {
            if (TempData["NumeroPedido"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
    }
}
