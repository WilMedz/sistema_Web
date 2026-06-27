namespace SISTEMA_WEB___TIENDA.Models;

public class CarritoItem
{
    public int VarianteId { get; set; }      // <-- NUEVO: Guardamos el ID de la variante
    public int PrendaId { get; set; }        // Para referencia
    public string NombrePrenda { get; set; } = null!;
    public string Talla { get; set; } = null!;      // <-- NUEVO
    public string Color { get; set; } = null!;      // <-- NUEVO
    public string? ImagenUrl { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }

    public decimal Total => Precio * Cantidad;
}