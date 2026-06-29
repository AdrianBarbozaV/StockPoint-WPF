using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    // Implementación mock — reemplazar por HttpClient cuando la API esté lista
    public class ProductoService : IProductoService
    {
        private readonly List<Producto> _store =
        [
            new() { ProductId = 1, CodigoBarra = "P001", NombreEtiqueta = "Laptop Lenovo",    Descripcion = "Laptop i5 16GB",   PrecioNeto = 450000, TieneImpuesto = true,  ImpuestoValorAgregado = 13, ExistenciaEnStock = 10, ExistenciaLimiteParaAlerta = 3,  PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 2, CodigoBarra = "P002", NombreEtiqueta = "Mouse Logitech",   Descripcion = "Mouse inalámbrico", PrecioNeto = 12000,  TieneImpuesto = true,  ImpuestoValorAgregado = 13, ExistenciaEnStock = 50, ExistenciaLimiteParaAlerta = 10, PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 3, CodigoBarra = "P003", NombreEtiqueta = "Cable HDMI 2m",    Descripcion = "Cable HDMI 4K",    PrecioNeto = 4500,   TieneImpuesto = false, ImpuestoValorAgregado = 0,  ExistenciaEnStock = 2,  ExistenciaLimiteParaAlerta = 5,  PuedeVenderse = true, PuedeComprarse = true },
            new() { ProductId = 4, CodigoBarra = "P004", NombreEtiqueta = "Teclado mecánico", Descripcion = "Teclado RGB",      PrecioNeto = 35000,  TieneImpuesto = true,  ImpuestoValorAgregado = 13, ExistenciaEnStock = 0,  ExistenciaLimiteParaAlerta = 5,  PuedeVenderse = true, PuedeComprarse = false },
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
            if (index >= 0) _store[index] = producto;
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
