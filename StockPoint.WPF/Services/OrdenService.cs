using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    public class OrdenService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = AppConfig.GetApiBaseUrl();

        public OrdenService()
        {
            if (client.BaseAddress == null)
                client.BaseAddress = new System.Uri(baseUrl);
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            var result = await client.GetFromJsonAsync<List<Cliente>>("api/clientes");
            return result ?? new List<Cliente>();
        }

        public async Task<List<Producto>> BuscarProductosAsync(string query)
        {
            var result = await client.GetFromJsonAsync<List<Producto>>($"api/productos?search={query}");
            return result ?? new List<Producto>();
        }

        public async Task ProcesarOrdenAsync(Orden orden)
        {
            var response = await client.PostAsJsonAsync("api/ordenes", orden);
            response.EnsureSuccessStatusCode();
        }
    }
}
