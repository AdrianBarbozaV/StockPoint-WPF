using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    public class OrdenService
    {
        private static readonly HttpClient _client = new();
        private static bool _initialized = false;
        private static readonly object _lock = new();

        public OrdenService()
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

        public async Task<List<Cliente>> GetClientesAsync()
        {
            var result = await _client.GetFromJsonAsync<List<Cliente>>("api/clientes");
            return result ?? [];
        }

        // Endpoint real: GET /api/productos/buscar?termino={query}
        public async Task<List<Producto>> BuscarProductosAsync(string query)
        {
            var encoded = Uri.EscapeDataString(query);
            var result  = await _client.GetFromJsonAsync<List<Producto>>($"api/productos/buscar?termino={encoded}");
            return result ?? [];
        }

        // Endpoint real: GET /api/ordenes/{numero}. Devuelve null si la orden no existe (404).
        public async Task<OrdenConsulta?> ObtenerOrdenAsync(int numero)
        {
            var response = await _client.GetAsync($"api/ordenes/{numero}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrdenConsulta>();
        }

        // El API espera CrearOrdenRequest: { fkCliente, fkEmpleado, detalles:[{fkProducto, cantidad, descuento}] }
        // El WPF no tiene selección de empleado → se usa empleado id=1 (primer empleado del seed).
        public async Task ProcesarOrdenAsync(Orden orden)
        {
            var request = new
            {
                fkCliente  = orden.ClienteId,
                fkEmpleado = 1,
                detalles   = orden.Detalles.Select(d => new
                {
                    fkProducto = d.ProductoId,
                    cantidad   = d.Cantidad,
                    descuento  = 0m
                }).ToList()
            };

            var response = await _client.PostAsJsonAsync("api/ordenes", request);

            if (!response.IsSuccessStatusCode)
            {
                // Intenta extraer el mensaje de error del API para mostrarlo al usuario.
                var body = await response.Content.ReadAsStringAsync();
                string msg;
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    msg = doc.RootElement.TryGetProperty("mensaje", out var m)
                        ? m.GetString() ?? body
                        : body;
                }
                catch { msg = body; }

                throw new HttpRequestException($"Error al procesar la orden: {msg}",
                    null, response.StatusCode);
            }
        }
    }
}
