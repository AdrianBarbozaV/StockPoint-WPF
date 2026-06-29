using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using StockPoint.WPF.Models;
using StockPoint.WPF.Services;

namespace StockPoint.WPF.ViewModels
{
    public partial class OrdenViewModel : ObservableObject
    {
        private readonly OrdenService ordenService;

        [ObservableProperty]
        private ObservableCollection<Cliente> clientes = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcesarOrdenCommand))]
        private Cliente? clienteSeleccionado;

        [ObservableProperty]
        private string busquedaProducto = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Producto> resultadosBusqueda = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AgregarProductoCommand))]
        private Producto? productoSeleccionado;

        [ObservableProperty]
        private ObservableCollection<OrdenDetalle> detalles = new();

        [ObservableProperty]
        private decimal totalImpuesto;

        [ObservableProperty]
        private decimal totalVenta;

        public System.DateTime FechaOrden { get; } = System.DateTime.Now;

        public OrdenViewModel()
        {
            ordenService = new OrdenService();
            _ = CargarClientesAsync();
        }

        private async Task CargarClientesAsync()
        {
            var lista = await ordenService.GetClientesAsync();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        }

        [RelayCommand]
        private async Task BuscarProducto()
        {
            if (string.IsNullOrWhiteSpace(BusquedaProducto)) return;
            var resultados = await ordenService.BuscarProductosAsync(BusquedaProducto);
            ResultadosBusqueda.Clear();
            foreach (var p in resultados) ResultadosBusqueda.Add(p);
        }

        private bool CanAgregarProducto() => ProductoSeleccionado != null;

        [RelayCommand(CanExecute = nameof(CanAgregarProducto))]
        private void AgregarProducto()
        {
            var existente = Detalles.FirstOrDefault(d => d.ProductoId == ProductoSeleccionado!.Id);
            if (existente != null) { existente.Cantidad++; }
            else
            {
                Detalles.Add(new OrdenDetalle
                {
                    ProductoId = ProductoSeleccionado!.ProductoId,
                    Codigo = ProductoSeleccionado.Codigo,
                    NombreProducto = ProductoSeleccionado.Nombre,
                    PrecioUnitario = ProductoSeleccionado.PrecioUnitario,
                    TieneImpuesto = ProductoSeleccionado.TieneImpuesto,
                    Cantidad = 1
                });
            }
            RecalcularTotales();
        }

        [RelayCommand]
        private void Remover(OrdenDetalle detalle)
        {
            Detalles.Remove(detalle);
            RecalcularTotales();
        }

        [RelayCommand]
        private void Incrementar(OrdenDetalle detalle)
        {
            detalle.Cantidad++;
            RecalcularTotales();
        }

        [RelayCommand]
        private void Decrementar(OrdenDetalle detalle)
        {
            if (detalle.Cantidad <= 1) { Remover(detalle); return; }
            detalle.Cantidad--;
            RecalcularTotales();
        }

        private bool CanProcesarOrden() => ClienteSeleccionado != null && Detalles.Count > 0;

        [RelayCommand(CanExecute = nameof(CanProcesarOrden))]
        private async Task ProcesarOrden()
        {
            var orden = new Orden
            {
                ClienteId = ClienteSeleccionado!.ClienteId,
                Fecha = FechaOrden,
                Detalles = Detalles.ToList()
            };

            try
            {
                await ordenService.ProcesarOrdenAsync(orden);
                MessageBox.Show("Orden procesada correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Detalles.Clear();
                RecalcularTotales();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al procesar la orden: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ponytail: IVA 13% fijo, confirmar con Joel cuando defina el campo en la API
        private void RecalcularTotales()
        {
            const decimal tasaIva = 0.13m;
            TotalImpuesto = Detalles.Where(d => d.TieneImpuesto).Sum(d => d.Subtotal * tasaIva);
            TotalVenta = Detalles.Sum(d => d.Subtotal) + TotalImpuesto;
        }
    }
}
