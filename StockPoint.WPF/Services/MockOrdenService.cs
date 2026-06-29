using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    // ponytail: swap por OrdenService en OrdenViewModel cuando Joel tenga la API
    public class MockOrdenService
    {
        public Task<List<Cliente>> GetClientesAsync() => Task.FromResult(new List<Cliente>
        {
            new() { ClienteId = 1, Nombre = "Carlos Ramírez", Cedula = "1-0801-0123" },
            new() { ClienteId = 2, Nombre = "María Solís",    Cedula = "2-0345-0678" },
            new() { ClienteId = 3, Nombre = "Jorge Ureña",    Cedula = "3-0567-0910" },
        });

        public Task<List<Producto>> BuscarProductosAsync(string query) => Task.FromResult(new List<Producto>
        {
            new() { ProductoId = 1, Codigo = "P001", Nombre = "Laptop Lenovo",   PrecioUnitario = 450000, TieneImpuesto = true,  Stock = 10 },
            new() { ProductoId = 2, Codigo = "P002", Nombre = "Mouse Logitech",  PrecioUnitario = 12000,  TieneImpuesto = true,  Stock = 50 },
            new() { ProductoId = 3, Codigo = "P003", Nombre = "Cable HDMI 2m",   PrecioUnitario = 4500,   TieneImpuesto = false, Stock = 30 },
            new() { ProductoId = 4, Codigo = "P004", Nombre = "Teclado mecánico",PrecioUnitario = 35000,  TieneImpuesto = true,  Stock = 15 },
        });

        public Task ProcesarOrdenAsync(Orden orden) => Task.CompletedTask;
    }
}
