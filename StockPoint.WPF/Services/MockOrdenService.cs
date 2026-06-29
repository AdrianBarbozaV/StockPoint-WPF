using System.Collections.Generic;
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
            new() { ProductId = 1, CodigoBarra = "P001", NombreEtiqueta = "Laptop Lenovo",    PrecioNeto = 450000, TieneImpuesto = true,  ExistenciaEnStock = 10 },
            new() { ProductId = 2, CodigoBarra = "P002", NombreEtiqueta = "Mouse Logitech",   PrecioNeto = 12000,  TieneImpuesto = true,  ExistenciaEnStock = 50 },
            new() { ProductId = 3, CodigoBarra = "P003", NombreEtiqueta = "Cable HDMI 2m",    PrecioNeto = 4500,   TieneImpuesto = false, ExistenciaEnStock = 30 },
            new() { ProductId = 4, CodigoBarra = "P004", NombreEtiqueta = "Teclado mecánico", PrecioNeto = 35000,  TieneImpuesto = true,  ExistenciaEnStock = 15 },
        });

        public Task ProcesarOrdenAsync(Orden orden) => Task.CompletedTask;
    }
}
