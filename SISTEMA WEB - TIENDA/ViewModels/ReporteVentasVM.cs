namespace SISTEMA_WEB___TIENDA.ViewModels
{
    public class ReporteVentasVM
    {
        public string Mes { get; set; }
        public int NumeroMes { get; set; }
        public int Anio { get; set; }
        public decimal MontoTotal { get; set; }
        public int TotalProductos { get; set; }
        public int TotalPedidos { get; set; }
    }
}