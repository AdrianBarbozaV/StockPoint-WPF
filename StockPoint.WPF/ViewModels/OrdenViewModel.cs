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
        private decimal subtotalSinImpuesto;

        [ObservableProperty]
        private decimal totalImpuesto;

        [ObservableProperty]
        private decimal totalVenta;

        [ObservableProperty]
        private string notas = string.Empty;

        public System.DateTime FechaOrden { get; } = System.DateTime.Now;

        public OrdenViewModel()
        {
            ordenService = new OrdenService();
            _ = CargarClientesAsync();
        }

        private async Task CargarClientesAsync()
        {
            try
            {
                var lista = await ordenService.GetClientesAsync();
                Clientes.Clear();
                foreach (var c in lista) Clientes.Add(c);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"No se pudo cargar la lista de clientes: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private async Task BuscarProducto()
        {
            if (string.IsNullOrWhiteSpace(BusquedaProducto)) return;
            try
            {
                var resultados = await ordenService.BuscarProductosAsync(BusquedaProducto);
                ResultadosBusqueda.Clear();
                foreach (var p in resultados) ResultadosBusqueda.Add(p);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanAgregarProducto() => ProductoSeleccionado != null;

        [RelayCommand(CanExecute = nameof(CanAgregarProducto))]
        private void AgregarProducto()
        {
            var existente = Detalles.FirstOrDefault(d => d.ProductoId == ProductoSeleccionado!.ProductId);
            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                Detalles.Add(new OrdenDetalle
                {
                    ProductoId             = ProductoSeleccionado!.ProductId,
                    Codigo                 = ProductoSeleccionado.CodigoBarra,
                    NombreProducto         = ProductoSeleccionado.NombreEtiqueta,
                    PrecioUnitario         = ProductoSeleccionado.PrecioNeto,
                    TieneImpuesto          = ProductoSeleccionado.TieneImpuesto,
                    ImpuestoValorAgregado  = ProductoSeleccionado.ImpuestoValorAgregado,
                    Cantidad               = 1
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
                Fecha     = FechaOrden,
                Detalles  = Detalles.ToList()
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
                MessageBox.Show(ex.Message, "Error al procesar la orden",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecalcularTotales()
        {
            SubtotalSinImpuesto = Detalles.Sum(d => d.Subtotal);
            TotalImpuesto       = Detalles.Sum(d => d.ImpuestoLinea);
            TotalVenta          = SubtotalSinImpuesto + TotalImpuesto;
        }
    }
}
