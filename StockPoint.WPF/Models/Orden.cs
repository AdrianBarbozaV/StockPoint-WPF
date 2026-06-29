using System;
using System.Collections.Generic;

namespace StockPoint.WPF.Models
{
    public class Orden
    {
        public int ClienteId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<OrdenDetalle> Detalles { get; set; } = new();
    }
}
