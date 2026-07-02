using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    public class ProductoService : IProductoService
    {
        private static readonly HttpClient _client = new();
        private static bool _initialized = false;
        private static readonly object _lock = new();

        public ProductoService()
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    _client.BaseAddress = new Uri(AppConfig.GetApiBaseUrl());
                    _initialized = true;
                }
            }
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            var result = await _client.GetFromJsonAsync<List<Producto>>("api/productos");
            return result ?? [];
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            try
            {
                return await _client.GetFromJsonAsync<Producto>($"api/productos/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<Producto>> BuscarAsync(string termino)
        {
            var encoded = Uri.EscapeDataString(termino);
            var result  = await _client.GetFromJsonAsync<List<Producto>>($"api/productos/buscar?termino={encoded}");
            return result ?? [];
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            // La API espera ProductoRequest con IdsCategoria en lugar de objetos Categoria completos.
            var request = BuildRequest(producto);
            var response = await _client.PostAsJsonAsync("api/productos", request);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<Producto>();
            return created!;
        }

        public async Task<Producto> UpdateAsync(Producto producto)
        {
            var request  = BuildRequest(producto);
            var response = await _client.PutAsJsonAsync($"api/productos/{producto.ProductId}", request);
            response.EnsureSuccessStatusCode();

            // PUT devuelve 204 sin body; hacemos un GET para devolver el objeto actualizado.
            return (await GetByIdAsync(producto.ProductId))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync($"api/productos/{id}");

            if (response.IsSuccessStatusCode) return true;

            // 400 = el producto tiene órdenes asociadas; relanzamos con el mensaje del API.
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                using var doc = await JsonDocument.ParseAsync(
                    await response.Content.ReadAsStreamAsync());
                var msg = doc.RootElement.TryGetProperty("mensaje", out var m)
                    ? m.GetString() ?? "No se puede eliminar el producto."
                    : "No se puede eliminar el producto.";
                throw new InvalidOperationException(msg);
            }

            response.EnsureSuccessStatusCode(); // 404 u otro error inesperado
            return false;
        }

        // GET /api/productos/{id}/ordenes → órdenes en las que se vendió el producto.
        public async Task<List<OrdenPorProducto>> GetOrdenesPorProductoAsync(int id)
        {
            var result = await _client.GetFromJsonAsync<List<OrdenPorProducto>>($"api/productos/{id}/ordenes");
            return result ?? [];
        }

        // Construye el DTO que espera la API (ProductoRequest con IdsCategoria).
        private static object BuildRequest(Producto p) => new
        {
            nombreEtiqueta             = p.NombreEtiqueta,
            descripcion                = p.Descripcion,
            existenciaEnStock          = p.ExistenciaEnStock,
            existenciaLimiteParaAlerta = p.ExistenciaLimiteParaAlerta,
            precioNeto                 = p.PrecioNeto,
            precioMinimo               = p.PrecioMinimo,
            precioMinimoConImpuesto    = p.PrecioMinimo,   // el WPF no tiene este campo, usar precioMinimo como fallback
            tieneImpuesto              = p.TieneImpuesto,
            impuestoValorAgregado      = p.ImpuestoValorAgregado,
            impuestoLocal1             = 0m,
            impuestoLocal2             = 0m,
            puedeVenderse              = p.PuedeVenderse,
            puedeComprarse             = p.PuedeComprarse,
            codigoBarra                = p.CodigoBarra,
            idsCategoria               = Array.Empty<int>()  // gestión de categorías no incluida en la UI WPF
        };
    }
}
