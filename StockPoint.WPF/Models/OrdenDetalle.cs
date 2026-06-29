using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockPoint.WPF.Models
{
    public class OrdenDetalle : INotifyPropertyChanged
    {
        private int productoId;
        private string codigo = string.Empty;
        private string nombreProducto = string.Empty;
        private decimal precioUnitario;
        private bool tieneImpuesto;
        private int cantidad;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ProductoId
        {
            get => productoId;
            set { if (productoId != value) { productoId = value; OnPropertyChanged(); } }
        }

        public string Codigo
        {
            get => codigo;
            set { if (codigo != value) { codigo = value; OnPropertyChanged(); } }
        }

        public string NombreProducto
        {
            get => nombreProducto;
            set { if (nombreProducto != value) { nombreProducto = value; OnPropertyChanged(); } }
        }

        public decimal PrecioUnitario
        {
            get => precioUnitario;
            set { if (precioUnitario != value) { precioUnitario = value; OnPropertyChanged(); } }
        }

        public bool TieneImpuesto
        {
            get => tieneImpuesto;
            set { if (tieneImpuesto != value) { tieneImpuesto = value; OnPropertyChanged(); } }
        }

        public int Cantidad
        {
            get => cantidad;
            set { if (cantidad != value) { cantidad = value; OnPropertyChanged(); OnPropertyChanged(nameof(Subtotal)); } }
        }

        public decimal Subtotal => PrecioUnitario * Cantidad;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
