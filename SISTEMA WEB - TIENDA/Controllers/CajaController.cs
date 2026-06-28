using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Extensions;
using SISTEMA_WEB___TIENDA.Models;
using SISTEMA_WEB___TIENDA.ViewModels;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Cajero,Administrador")]
    public class CajaController : Controller
    {
        private readonly AppDbContext _context;
        private const string SessionKey = "CarritoAleana";

        public CajaController(AppDbContext context) => _context = context;

        // ── PANEL PRINCIPAL ──────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            var hoy = DateTime.Today;

            // Caja abierta del cajero actual
            var cajaAbierta = cajeroId.HasValue
                ? await _context.RegistrosCaja
                    .FirstOrDefaultAsync(r => r.ClientesId == cajeroId.Value && r.FechaCierre == null)
                : null;

            // Ventas del día de ESTE cajero
            var pedidosHoy = cajeroId.HasValue
                ? await _context.Pedidos
                    .Include(p => p.Estado)
                    .Include(p => p.MetodoPago)
                    .Include(p => p.Clientes)
                    .Where(p => p.FechaPedido.Date == hoy && p.ClientesId != cajeroId.Value)
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync()
                : new List<Pedido>();

            // Estadísticas globales del día
            var todosHoy = await _context.Pedidos
                .Include(p => p.Estado)
                .Where(p => p.FechaPedido.Date == hoy)
                .ToListAsync();

            ViewBag.CajaAbierta = cajaAbierta;
            ViewBag.TotalVentasHoy = todosHoy.Sum(p => p.TotalCompra);
            ViewBag.TotalPedidosHoy = todosHoy.Count;
            ViewBag.PedidosHoy = pedidosHoy;
            ViewBag.CajeroNombre = HttpContext.Session.GetString("UsuarioNombre") ?? "";

            return View();
        }

        // ── APERTURA DE CAJA ─────────────────────────────────────────
        [HttpGet]
        [Authorize(Roles = "Cajero")]
        public IActionResult AbrirCaja() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Cajero")]
        public async Task<IActionResult> AbrirCaja(decimal montoInicial)
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            if (cajeroId == null) return RedirectToAction("Login", "Login");

            var yaAbierta = await _context.RegistrosCaja
                .AnyAsync(r => r.ClientesId == cajeroId.Value && r.FechaCierre == null);

            if (yaAbierta)
            {
                TempData["ErrorCaja"] = "Ya tienes una caja abierta.";
                return RedirectToAction(nameof(Index));
            }

            _context.RegistrosCaja.Add(new RegistroCaja
            {
                ClientesId = cajeroId.Value,
                FechaApertura = DateTime.Now,
                MontoInicial = montoInicial
            });
            await _context.SaveChangesAsync();
            TempData["OK"] = "Caja abierta correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── CIERRE DE CAJA ───────────────────────────────────────────
        [HttpGet]
        [Authorize(Roles = "Cajero")]
        public async Task<IActionResult> CerrarCaja()
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            var caja = await _context.RegistrosCaja
                .FirstOrDefaultAsync(r => r.ClientesId == cajeroId && r.FechaCierre == null);

            if (caja == null)
            {
                TempData["ErrorCaja"] = "No tienes una caja abierta.";
                return RedirectToAction(nameof(Index));
            }

            var totalVentas = await _context.Pedidos
                .Where(p => p.FechaPedido >= caja.FechaApertura)
                .SumAsync(p => (decimal?)p.TotalCompra) ?? 0;

            ViewBag.Caja = caja;
            ViewBag.TotalVentas = totalVentas;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Cajero")]
        public async Task<IActionResult> CerrarCaja(decimal montoFinal)
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            var caja = await _context.RegistrosCaja
                .FirstOrDefaultAsync(r => r.ClientesId == cajeroId && r.FechaCierre == null);

            if (caja == null) return RedirectToAction(nameof(Index));

            caja.FechaCierre = DateTime.Now;
            caja.MontoFinal = montoFinal;
            await _context.SaveChangesAsync();
            TempData["OK"] = "Caja cerrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── NUEVA VENTA ──────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> NuevaVenta()
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            var cajaAbierta = cajeroId.HasValue
                ? await _context.RegistrosCaja
                    .FirstOrDefaultAsync(r => r.ClientesId == cajeroId.Value && r.FechaCierre == null)
                : null;

            if (cajaAbierta == null)
            {
                TempData["ErrorCaja"] = "Debes abrir la caja antes de registrar una venta.";
                return RedirectToAction(nameof(Index));
            }

            // Limpiar carrito anterior
            HttpContext.Session.Remove(SessionKey);

            var variantes = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .Where(v => v.Stock > 0)
                .OrderBy(v => v.Prenda.NombrePrenda)
                .ToListAsync();

            ViewBag.MetodosPago = new SelectList(
                await _context.MetodosPago.ToListAsync(),
                "MetodoPagoId", "DescripcionPago");

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            ViewBag.Carrito = carrito;
            ViewBag.GranTotal = carrito.Sum(i => i.Total);

            return View(variantes);
        }
        // ── GENERAR PEDIDO DIRECTO (sin cliente registrado) ──────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarPedidoDirecto(string NombreCliente,
            string CalleAvenida, string Distrito, string Referencia, int MetodoPagoId)
        {
            var cajeroId = HttpContext.Session.GetInt32("ClienteId");
            if (cajeroId == null) return RedirectToAction("Login", "Login");

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            if (!carrito.Any()) return RedirectToAction(nameof(NuevaVenta));

            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear cliente temporal con el nombre ingresado
                var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Cliente");
                var ciudadDefault = await _context.Ciudades.FirstAsync();

                var clienteTemp = new Clientes
                {
                    Nombres = NombreCliente,
                    CorreoElectronico = $"venta_{DateTime.Now.Ticks}@caja.local",
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    CiudadId = ciudadDefault.CiudadId,
                    RolId = rolCliente?.RolId ?? 2,
                    FechaNacimiento = new DateTime(1990, 1, 1)
                };
                _context.Clientes.Add(clienteTemp);
                await _context.SaveChangesAsync();

                var direccion = new DireccionEnvio
                {
                    CalleAvenida = CalleAvenida,
                    Distrito = Distrito,
                    Referencia = Referencia,
                    ClientesId = clienteTemp.ClienteId
                };
                _context.DireccionesEnvio.Add(direccion);
                await _context.SaveChangesAsync();

                var estado = await _context.EstadosPedido
                    .FirstOrDefaultAsync(e => e.NombreEstado == "Pendiente")
                    ?? await _context.EstadosPedido.FirstAsync();

                var pedido = new Pedido
                {
                    FechaPedido = DateTime.Now,
                    TotalCompra = carrito.Sum(i => i.Total),
                    ClientesId = clienteTemp.ClienteId,
                    DireccionId = direccion.DireccionId,
                    MetodoPagoId = MetodoPagoId,
                    EstadoId = estado.EstadoId
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                foreach (var item in carrito)
                {
                    var variante = await _context.VariantesPrenda.FindAsync(item.VarianteId);
                    if (variante == null) throw new Exception($"Variante de '{item.NombrePrenda}' no encontrada.");
                    if (item.Cantidad > variante.Stock) throw new Exception($"Stock insuficiente para '{item.NombrePrenda}'.");

                    variante.Stock -= item.Cantidad;
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

                TempData["OK"] = $"Venta registrada correctamente para {NombreCliente}.";
                return RedirectToAction(nameof(Comprobante), new { id = pedido.PedidoId });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                TempData["ErrorVenta"] = "Error: " + ex.Message;
                return RedirectToAction(nameof(NuevaVenta));
            }
        }
        // ── SELECCIONAR PRODUCTOS ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> SeleccionarProductos()
        {
            var clienteId = HttpContext.Session.GetInt32("CajaClienteId");
            if (clienteId == null) return RedirectToAction(nameof(NuevaVenta));

            var variantes = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .Where(v => v.Stock > 0)
                .OrderBy(v => v.Prenda.NombrePrenda)
                .ToListAsync();

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();

            ViewBag.ClienteNombre = HttpContext.Session.GetString("CajaClienteNombre");
            ViewBag.Carrito = carrito;
            ViewBag.GranTotal = carrito.Sum(i => i.Total);
            return View(variantes);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int varianteId, int cantidad = 1)
        {
            var clienteId = HttpContext.Session.GetInt32("CajaClienteId");
            if (clienteId == null) return RedirectToAction(nameof(NuevaVenta));

            var variante = await _context.VariantesPrenda
                .Include(v => v.Prenda)
                .FirstOrDefaultAsync(v => v.VarianteId == varianteId);

            if (variante == null) return NotFound();

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();

            var item = carrito.FirstOrDefault(i => i.VarianteId == varianteId);
            int total = cantidad + (item?.Cantidad ?? 0);

            if (total > variante.Stock)
            {
                TempData["ErrorStock"] = $"Stock insuficiente. Disponible: {variante.Stock}";
                return RedirectToAction(nameof(SeleccionarProductos));
            }

            decimal precio = (variante.Prenda.PrecioOferta.HasValue && variante.Prenda.PrecioOferta > 0)
                ? variante.Prenda.PrecioOferta.Value : variante.Prenda.PrecioLista;

            if (item != null)
                item.Cantidad += cantidad;
            else
                carrito.Add(new CarritoItem
                {
                    VarianteId = variante.VarianteId,
                    PrendaId = variante.PrendaId,
                    NombrePrenda = variante.Prenda.NombrePrenda,
                    Talla = variante.Talla,
                    Color = variante.Color,
                    ImagenUrl = variante.Prenda.ImagenPrincipalUrl,
                    Precio = precio,
                    Cantidad = cantidad
                });

            HttpContext.Session.SetObject(SessionKey, carrito);
            return RedirectToAction(nameof(SeleccionarProductos));
        }

        [HttpPost]
        public IActionResult QuitarProducto(int varianteId)
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            var item = carrito.FirstOrDefault(i => i.VarianteId == varianteId);
            if (item != null) carrito.Remove(item);
            HttpContext.Session.SetObject(SessionKey, carrito);
            return RedirectToAction(nameof(SeleccionarProductos));
        }

        // ── CONFIRMAR VENTA ──────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ConfirmarVenta()
        {
            var clienteId = HttpContext.Session.GetInt32("CajaClienteId");
            if (clienteId == null) return RedirectToAction(nameof(NuevaVenta));

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            if (!carrito.Any()) return RedirectToAction(nameof(SeleccionarProductos));

            ViewBag.ClienteNombre = HttpContext.Session.GetString("CajaClienteNombre");
            ViewBag.Carrito = carrito;
            ViewBag.GranTotal = carrito.Sum(i => i.Total);
            ViewBag.MetodosPago = new SelectList(
                await _context.MetodosPago.ToListAsync(),
                "MetodoPagoId", "DescripcionPago");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarPedido(int MetodoPagoId, string CalleAvenida,
            string Distrito, string Referencia)
        {
            var clienteId = HttpContext.Session.GetInt32("CajaClienteId");
            if (clienteId == null) return RedirectToAction(nameof(NuevaVenta));

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(SessionKey)
                          ?? new List<CarritoItem>();
            if (!carrito.Any()) return RedirectToAction(nameof(SeleccionarProductos));

            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                var direccion = new DireccionEnvio
                {
                    CalleAvenida = CalleAvenida,
                    Distrito = Distrito,
                    Referencia = Referencia,
                    ClientesId = clienteId.Value
                };
                _context.DireccionesEnvio.Add(direccion);
                await _context.SaveChangesAsync();

                var estado = await _context.EstadosPedido
                    .FirstOrDefaultAsync(e => e.NombreEstado == "Pendiente")
                    ?? await _context.EstadosPedido.FirstAsync();

                var pedido = new Pedido
                {
                    FechaPedido = DateTime.Now,
                    TotalCompra = carrito.Sum(i => i.Total),
                    ClientesId = clienteId.Value,
                    DireccionId = direccion.DireccionId,
                    MetodoPagoId = MetodoPagoId,
                    EstadoId = estado.EstadoId
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                foreach (var item in carrito)
                {
                    var variante = await _context.VariantesPrenda
                        .FirstOrDefaultAsync(v => v.VarianteId == item.VarianteId);
                    if (variante == null)
                        throw new Exception($"Variante de '{item.NombrePrenda}' no encontrada.");
                    if (item.Cantidad > variante.Stock)
                        throw new Exception($"Stock insuficiente para '{item.NombrePrenda}'.");

                    variante.Stock -= item.Cantidad;
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
                HttpContext.Session.Remove("CajaClienteId");
                HttpContext.Session.Remove("CajaClienteNombre");

                TempData["PedidoGenerado"] = pedido.PedidoId;
                return RedirectToAction(nameof(Comprobante), new { id = pedido.PedidoId });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                TempData["ErrorVenta"] = "Error: " + ex.Message;
                return RedirectToAction(nameof(ConfirmarVenta));
            }
        }

        // ── COMPROBANTE ──────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.MetodoPago)
                .Include(p => p.Estado)
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }

        // ── CONSULTAR / CAMBIAR ESTADO PEDIDO ───────────────────────
        [HttpGet]
        public async Task<IActionResult> ConsultarPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.MetodoPago)
                .Include(p => p.Estado)
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();

            ViewBag.Estados = new SelectList(
                await _context.EstadosPedido.ToListAsync(),
                "EstadoId", "NombreEstado", pedido.EstadoId);

            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoPedido(int pedidoId, int estadoId)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null) return NotFound();
            pedido.EstadoId = estadoId;
            await _context.SaveChangesAsync();
            TempData["OK"] = "Estado actualizado correctamente.";
            return RedirectToAction(nameof(ConsultarPedido), new { id = pedidoId });
        }

        // ── VENTAS DEL DÍA ───────────────────────────────────────────
        [HttpGet]
        [Authorize(Roles = "Cajero")]
        public async Task<IActionResult> VentasDelDia()
        {
            var hoy = DateTime.Today;
            var pedidos = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Where(p => p.FechaPedido.Date == hoy)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            ViewBag.TotalDia = pedidos.Sum(p => p.TotalCompra);
            ViewBag.TotalPedidos = pedidos.Count;
            return View(pedidos);
        }
    }
}