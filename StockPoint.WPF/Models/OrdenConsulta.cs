using System;
using System.Collections.Generic;

namespace StockPoint.WPF.Models
{
    // Respuesta de GET /api/ordenes/{numero}. Los nombres coinciden (case-insensitive)
    // con el JSON camelCase que devuelve la API.
    public class OrdenConsulta
    {
        public int NumeroOrden { get; set; }
        public int FkCliente { get; set; }
        public int FkEmpleado { get; set; }
        public DateTime FechaOrden { get; set; }
        public decimal TotalOrden { get; set; }
        public decimal Impuesto { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public List<LineaOrdenConsulta> Detalles { get; set; } = new();

        public decimal Subtotal => TotalOrden - Impuesto;
    }

    public class LineaOrdenConsulta
    {
        public int FkProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Descuento { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioSubtotal { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string? CodigoProducto { get; set; }
    }
}
