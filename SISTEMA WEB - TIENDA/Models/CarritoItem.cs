namespace SISTEMA_WEB___TIENDA.Models;

public class CarritoItem
{
    public int PrendaId { get; set; }
    public string NombrePrenda { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }

    //  calculo automáticamente por fila
    public decimal Total => Precio * Cantidad;
}