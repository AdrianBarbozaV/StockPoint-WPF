using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    // Implementación mock — reemplazar por HttpClient cuando la API esté lista
    public class ProductoService : IProductoService
    {
        private readonly List<Producto> _store =
        [
            new() { ProductId = 1, CodigoBarra = "COD-001", NombreEtiqueta = "Coca-Cola 500ml",   Descripcion = "Refresco",           PrecioNeto = 850,  TieneImpuesto = true,  ImpuestoValorAgregado = 13, ExistenciaEnStock = 50,  ExistenciaLimiteParaAlerta = 10, PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 2, CodigoBarra = "COD-002", NombreEtiqueta = "Agua Purificada 1L", Descripcion = "Agua embotellada",   PrecioNeto = 500,  TieneImpuesto = false, ImpuestoValorAgregado = 0,  ExistenciaEnStock = 120, ExistenciaLimiteParaAlerta = 20, PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 3, CodigoBarra = "COD-003", NombreEtiqueta = "Pan de molde",       Descripcion = "Pan blanco 500g",    PrecioNeto = 1200, TieneImpuesto = false, ImpuestoValorAgregado = 0,  ExistenciaEnStock = 30,  ExistenciaLimiteParaAlerta = 5,  PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 4, CodigoBarra = "COD-004", NombreEtiqueta = "Jugo de naranja",    Descripcion = "Jugo natural 1L",    PrecioNeto = 1500, TieneImpuesto = true,  ImpuestoValorAgregado = 13, ExistenciaEnStock = 15,  ExistenciaLimiteParaAlerta = 5,  PuedeVenderse = true, PuedeComprarse = false },
        ];

        private int _nextId = 5;

        public Task<List<Producto>> GetAllAsync() =>
            Task.FromResult(_store.ToList());

        public Task<Producto?> GetByIdAsync(int id) =>
            Task.FromResult(_store.FirstOrDefault(p => p.ProductId == id));

        public Task<Producto> CreateAsync(Producto producto)
        {
            producto.ProductId = _nextId++;
            _store.Add(producto);
            return Task.FromResult(producto);
        }

        public Task<Producto> UpdateAsync(Producto producto)
        {
            var index = _store.FindIndex(p => p.ProductId == producto.ProductId);
            if (index >= 0)
                _store[index] = producto;
            return Task.FromResult(producto);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var item = _store.FirstOrDefault(p => p.ProductId == id);
            if (item is null) return Task.FromResult(false);
            _store.Remove(item);
            return Task.FromResult(true);
        }
    }
}
