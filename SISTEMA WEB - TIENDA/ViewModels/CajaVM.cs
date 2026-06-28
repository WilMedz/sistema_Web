namespace SISTEMA_WEB___TIENDA.ViewModels
{
    public class CajaVM
    {
        public int CajaId { get; set; }
        public string CajerNombre { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal? MontoFinal { get; set; }
        public bool EstaAbierta => FechaCierre == null;
        public decimal TotalVentas { get; set; }
        public int TotalPedidos { get; set; }
    }
}
