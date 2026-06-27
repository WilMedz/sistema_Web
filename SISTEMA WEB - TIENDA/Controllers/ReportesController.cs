using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.ViewModels;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;
        public ReportesController(AppDbContext context) => _context = context;

        public IActionResult Index() => RedirectToAction(nameof(Ventas));

        public async Task<IActionResult> Ventas()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Estado)
                .Where(p => p.Estado.NombreEstado != "Cancelado")
                .ToListAsync();

            var reporte = pedidos
                .GroupBy(p => new { p.FechaPedido.Year, p.FechaPedido.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Select(g => new ReporteVentasVM
                {
                    Anio = g.Key.Year,
                    NumeroMes = g.Key.Month,
                    Mes = new DateTime(g.Key.Year, g.Key.Month, 1)
                                        .ToString("MMMM yyyy",
                                            new System.Globalization.CultureInfo("es-PE")),
                    MontoTotal = g.Sum(p => p.TotalCompra),
                    TotalPedidos = g.Count(),
                    TotalProductos = g.Sum(p => p.Detalles.Sum(d => d.Cantidad))
                })
                .ToList();

            ViewBag.TotalGeneral = reporte.Sum(r => r.MontoTotal);
            ViewBag.PedidosTotal = reporte.Sum(r => r.TotalPedidos);
            ViewBag.ProductosTotal = reporte.Sum(r => r.TotalProductos);
            ViewBag.MejorMes = reporte.OrderByDescending(r => r.MontoTotal).FirstOrDefault()?.Mes ?? "—";
            ViewBag.MejorMonto = reporte.OrderByDescending(r => r.MontoTotal).FirstOrDefault()?.MontoTotal ?? 0;

            return View(reporte);
        }

        public async Task<IActionResult> VentasPorMetodoPago()
        {
            var metodos = await _context.MetodosPago
                .Include(m => m.Pedidos)
                    .ThenInclude(p => p.Clientes)
                .Include(m => m.Pedidos)
                    .ThenInclude(p => p.Estado)
                .ToListAsync();

            // Retorna la vista con los mismos datos
            return View(metodos);
        }
    }
}