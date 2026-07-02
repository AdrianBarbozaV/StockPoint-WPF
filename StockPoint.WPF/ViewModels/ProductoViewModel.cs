using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockPoint.WPF.Models;
using StockPoint.WPF.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace StockPoint.WPF.ViewModels
{
    public partial class ProductoViewModel : ObservableObject
    {
        private readonly IProductoService _service;

        [ObservableProperty]
        private ObservableCollection<Producto> _productos = [];

        [ObservableProperty]
        private ICollectionView? _productosView;

        [ObservableProperty]
        private Producto? _selectedProducto;

        [ObservableProperty]
        private Producto _form = new();

        [ObservableProperty]
        private bool _isFormVisible;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CloseButtonText))]
        private bool _isViewMode;

        [ObservableProperty]
        private string _formTitle = string.Empty;

        public string CloseButtonText => IsViewMode ? "Cerrar" : "Cancelar";

        [ObservableProperty]
        private string _searchText = string.Empty;

        // Órdenes en las que se vendió el producto que se está visualizando.
        [ObservableProperty]
        private ObservableCollection<OrdenPorProducto> _ordenesAsociadas = [];

        [ObservableProperty]
        private bool _cargandoOrdenes;

        private bool _isNew;

        // ── Estadísticas ──────────────────────────────────────────────────
        public int TotalProductos => Productos.Count;
        public decimal ValorInventario => Productos.Sum(p => p.PrecioNeto * p.ExistenciaEnStock);
        public int ProductosBajoStock => Productos.Count(p =>
            p.ExistenciaEnStock == 0 ||
            (p.ExistenciaLimiteParaAlerta > 0 && p.ExistenciaEnStock <= p.ExistenciaLimiteParaAlerta));
        public bool HayAlertaStock => ProductosBajoStock > 0;

        public ProductoViewModel(IProductoService service)
        {
            _service = service;
        }

        partial void OnProductosChanged(ObservableCollection<Producto> value)
        {
            var view = CollectionViewSource.GetDefaultView(value);
            view.Filter = FiltrarProducto;
            ProductosView = view;
            value.CollectionChanged += (_, _) => ActualizarEstadisticas();
            ActualizarEstadisticas();
        }

        partial void OnSearchTextChanged(string value)
            => ProductosView?.Refresh();

        private bool FiltrarProducto(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            var p = (Producto)obj;
            return p.NombreEtiqueta.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || p.CodigoBarra.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        private void ActualizarEstadisticas()
        {
            OnPropertyChanged(nameof(TotalProductos));
            OnPropertyChanged(nameof(ValorInventario));
            OnPropertyChanged(nameof(ProductosBajoStock));
            OnPropertyChanged(nameof(HayAlertaStock));
        }

        // ── Comandos ──────────────────────────────────────────────────────
        [RelayCommand]
        private async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Productos = new ObservableCollection<Producto>(items);
        }

        [RelayCommand]
        private void New()
        {
            _isNew = true;
            Form = new Producto();
            FormTitle = "Nuevo producto";
            IsViewMode = false;
            IsFormVisible = true;
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void Edit()
        {
            _isNew = false;
            Form = CopiarDeSeleccionado();
            FormTitle = "Editar producto";
            IsViewMode = false;
            IsFormVisible = true;
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private async Task ViewAsync()
        {
            var id = SelectedProducto!.ProductId;
            Form = CopiarDeSeleccionado();
            FormTitle = "Detalles del producto";
            IsViewMode = true;
            IsFormVisible = true;

            OrdenesAsociadas = [];
            CargandoOrdenes = true;
            try
            {
                var ordenes = await _service.GetOrdenesPorProductoAsync(id);
                OrdenesAsociadas = new ObservableCollection<OrdenPorProducto>(ordenes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar las órdenes del producto: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                CargandoOrdenes = false;
            }
        }

        private Producto CopiarDeSeleccionado() => new()
        {
            ProductId                  = SelectedProducto!.ProductId,
            CodigoBarra                = SelectedProducto.CodigoBarra,
            NombreEtiqueta             = SelectedProducto.NombreEtiqueta,
            Descripcion                = SelectedProducto.Descripcion,
            PrecioNeto                 = SelectedProducto.PrecioNeto,
            PrecioMinimo               = SelectedProducto.PrecioMinimo,
            TieneImpuesto              = SelectedProducto.TieneImpuesto,
            ImpuestoValorAgregado      = SelectedProducto.ImpuestoValorAgregado,
            ExistenciaEnStock          = SelectedProducto.ExistenciaEnStock,
            ExistenciaLimiteParaAlerta = SelectedProducto.ExistenciaLimiteParaAlerta,
            PuedeVenderse              = SelectedProducto.PuedeVenderse,
            PuedeComprarse             = SelectedProducto.PuedeComprarse,
        };

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Form.NombreEtiqueta))
            {
                MessageBox.Show("El nombre del producto es obligatorio.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isNew)
            {
                var created = await _service.CreateAsync(Form);
                Productos.Add(created);
            }
            else
            {
                var updated = await _service.UpdateAsync(Form);
                var idx = Productos.IndexOf(Productos.First(p => p.ProductId == updated.ProductId));
                Productos[idx] = updated;
                SelectedProducto = updated;
            }

            IsFormVisible = false;
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private async Task DeleteAsync()
        {
            var confirm = MessageBox.Show(
                $"¿Eliminar \"{SelectedProducto!.NombreEtiqueta}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _service.DeleteAsync(SelectedProducto.ProductId);
                Productos.Remove(SelectedProducto);
                IsFormVisible = false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "No se puede eliminar",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void Cancel() => IsFormVisible = false;

        private bool HasSelection() => SelectedProducto is not null;

        partial void OnSelectedProductoChanged(Producto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            ViewCommand.NotifyCanExecuteChanged();
        }
    }
}
