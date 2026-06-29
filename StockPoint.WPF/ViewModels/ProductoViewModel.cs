using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockPoint.WPF.Models;
using StockPoint.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace StockPoint.WPF.ViewModels
{
    public partial class ProductoViewModel : ObservableObject
    {
        private readonly IProductoService _service;

        [ObservableProperty]
        private ObservableCollection<Producto> _productos = [];

        [ObservableProperty]
        private Producto? _selectedProducto;

        [ObservableProperty]
        private Producto _form = new();

        [ObservableProperty]
        private bool _isFormVisible;

        [ObservableProperty]
        private string _formTitle = string.Empty;

        private bool _isNew;

        public ProductoViewModel(IProductoService service)
        {
            _service = service;
        }

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
            IsFormVisible = true;
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void Edit()
        {
            _isNew = false;
            Form = new Producto
            {
                ProductId               = SelectedProducto!.ProductId,
                CodigoBarra             = SelectedProducto.CodigoBarra,
                NombreEtiqueta          = SelectedProducto.NombreEtiqueta,
                Descripcion             = SelectedProducto.Descripcion,
                PrecioNeto              = SelectedProducto.PrecioNeto,
                PrecioMinimo            = SelectedProducto.PrecioMinimo,
                TieneImpuesto           = SelectedProducto.TieneImpuesto,
                ImpuestoValorAgregado   = SelectedProducto.ImpuestoValorAgregado,
                ExistenciaEnStock       = SelectedProducto.ExistenciaEnStock,
                ExistenciaLimiteParaAlerta = SelectedProducto.ExistenciaLimiteParaAlerta,
                PuedeVenderse           = SelectedProducto.PuedeVenderse,
                PuedeComprarse          = SelectedProducto.PuedeComprarse,
            };
            FormTitle = "Editar producto";
            IsFormVisible = true;
        }

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

            await _service.DeleteAsync(SelectedProducto.ProductId);
            Productos.Remove(SelectedProducto);
            IsFormVisible = false;
        }

        [RelayCommand]
        private void Cancel()
        {
            IsFormVisible = false;
        }

        private bool HasSelection() => SelectedProducto is not null;

        partial void OnSelectedProductoChanged(Producto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }
    }
}
