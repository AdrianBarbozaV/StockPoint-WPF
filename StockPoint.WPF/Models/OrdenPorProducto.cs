using System;

namespace StockPoint.WPF.Models
{
    // Respuesta de GET /api/productos/{id}/ordenes. Los nombres coinciden
    // (case-insensitive) con el JSON camelCase que devuelve la API.
    // NumeroOrden se usa para consultar la orden completa en la vista de consulta.
    public class OrdenPorProducto
    {
        public int NumeroOrden { get; set; }
        public DateTime FechaOrden { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
