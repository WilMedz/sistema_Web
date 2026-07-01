using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISTEMA_WEB___TIENDA.Data;
using SISTEMA_WEB___TIENDA.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SISTEMA_WEB___TIENDA.Controllers
{
    [Authorize(Roles = "Administrador,Cajero")]
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;
        public PedidosController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? searchString)
        {
            var lista = await ObtenerPedidosFiltradosAsync(searchString);
            ViewBag.CurrentFilter = searchString;

            if (User.IsInRole("Administrador"))
                return View("IndexAdmin", lista);

            return View("IndexCajero", lista);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Include(p => p.Cajero)
                .Include(p => p.DireccionEnvio)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Variante)
                        .ThenInclude(v => v.Prenda)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpGet]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p => p.PedidoId == id);
            if (pedido == null) return NotFound();

            ViewBag.Estados = new SelectList(
                await _context.EstadosPedido.ToListAsync(),
                "EstadoId", "NombreEstado", pedido.EstadoId);

            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, int EstadoId)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.EstadoId = EstadoId;
            await _context.SaveChangesAsync();
            TempData["OK"] = "Estado del pedido actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();

            foreach (var detalle in pedido.Detalles)
            {
                var variante = await _context.VariantesPrenda.FindAsync(detalle.VarianteId);
                if (variante != null) variante.Stock += detalle.Cantidad;
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            TempData["OK"] = $"Pedido #{id:D6} eliminado y stock restaurado.";
            return RedirectToAction(nameof(Index));
        }

        //reportes de excel y pdf

        public async Task<IActionResult> ExportarExcel(string? searchString)
        {
            var pedidos = await ObtenerPedidosFiltradosAsync(searchString);

            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Pedidos y Ventas");

            hoja.Cell(1, 1).Value = "Reporte de Pedidos y Ventas";
            hoja.Range(1, 1, 1, 7).Merge().Style
                .Font.SetBold().Font.SetFontSize(14)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#4a1020"))
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            hoja.Cell(2, 1).Value = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Range(2, 1, 2, 7).Merge().Style.Font.SetItalic().Font.SetFontSize(9);

            int filaCab = 4;
            string[] cabeceras = { "N° Pedido", "Fecha", "Cliente", "Método Pago", "Estado", "Cajero", "Total (S/)" };
            for (int i = 0; i < cabeceras.Length; i++)
            {
                var celda = hoja.Cell(filaCab, i + 1);
                celda.Value = cabeceras[i];
                celda.Style.Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#6b1a2a"))
                    .Font.SetFontColor(XLColor.White);
            }

            int fila = filaCab + 1;
            foreach (var p in pedidos)
            {
                hoja.Cell(fila, 1).Value = "#" + p.PedidoId.ToString("D6");
                hoja.Cell(fila, 2).Value = p.FechaPedido.ToString("dd/MM/yyyy HH:mm");
                hoja.Cell(fila, 3).Value = p.Clientes?.Nombres ?? "-";
                hoja.Cell(fila, 4).Value = p.MetodoPago?.DescripcionPago ?? "-";
                hoja.Cell(fila, 5).Value = p.Estado?.NombreEstado ?? "-";
                hoja.Cell(fila, 6).Value = p.Cajero != null ? p.Cajero.Nombres : "Web";
                hoja.Cell(fila, 7).Value = p.TotalCompra;
                hoja.Cell(fila, 7).Style.NumberFormat.Format = "S/ #,##0.00";
                fila++;
            }

            hoja.Cell(fila, 6).Value = "TOTAL VENTAS:";
            hoja.Cell(fila, 6).Style.Font.SetBold();
            hoja.Cell(fila, 7).Value = pedidos.Sum(p => p.TotalCompra);
            hoja.Cell(fila, 7).Style.Font.SetBold().NumberFormat.Format = "S/ #,##0.00";

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            string nombreArchivo = $"Reporte_Pedidos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombreArchivo);
        }

        public async Task<IActionResult> ExportarPdf(string? searchString)
        {
            var pedidos = await ObtenerPedidosFiltradosAsync(searchString);
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Pedidos y Ventas").FontSize(16).Bold().FontColor("#4a1020");
                        col.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#6b1a2a");
                    });

                    page.Content().PaddingTop(10).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(1.2f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(2f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(1.2f);
                            c.RelativeColumn(1.3f);
                            c.RelativeColumn(1.2f);
                        });

                        tabla.Header(header =>
                        {
                            string[] cabeceras = { "N° Pedido", "Fecha", "Cliente", "Método Pago", "Estado", "Cajero", "Total" };
                            foreach (var texto in cabeceras)
                                header.Cell().Background("#4a1020").Padding(4).Text(texto).FontColor(Colors.White).Bold().FontSize(8);
                        });

                        foreach (var p in pedidos)
                        {
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text("#" + p.PedidoId.ToString("D6"));
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.FechaPedido.ToString("dd/MM/yyyy HH:mm"));
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.Clientes?.Nombres ?? "-");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.MetodoPago?.DescripcionPago ?? "-");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.Estado?.NombreEstado ?? "-");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.Cajero != null ? p.Cajero.Nombres : "Web");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"S/ {p.TotalCompra:N2}").Bold();
                        }
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#6b1a2a");
                        col.Item().AlignRight().Text(text =>
                        {
                            text.Span("TOTAL VENTAS: ").Bold();
                            text.Span($"S/ {pedidos.Sum(p => p.TotalCompra):N2}").Bold().FontColor("#6b1a2a");
                        });
                        col.Item().AlignCenter().PaddingTop(5).Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                });
            });

            var contenido = documento.GeneratePdf();
            string nombreArchivo = $"Reporte_Pedidos_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(contenido, "application/pdf", nombreArchivo);
        }


        //lógica de filtro entre Index y los reportes
         
        private async Task<List<Pedido>> ObtenerPedidosFiltradosAsync(string? searchString)
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Clientes)
                .Include(p => p.Estado)
                .Include(p => p.MetodoPago)
                .Include(p => p.Cajero)
                .AsQueryable();

            if (User.IsInRole("Cajero"))
            {
                var cajeroId = HttpContext.Session.GetInt32("ClienteId");
                if (cajeroId.HasValue)
                    pedidos = pedidos.Where(p => p.CajeroId == cajeroId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
                pedidos = pedidos.Where(p => p.PedidoId.ToString().Contains(searchString));

            return await pedidos.OrderByDescending(p => p.FechaPedido).ToListAsync();
        }
    }
}