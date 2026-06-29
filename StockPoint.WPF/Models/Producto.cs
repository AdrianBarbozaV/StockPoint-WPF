using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockPoint.WPF.Models
{
    public class Producto : INotifyPropertyChanged
    {
        private int productoId;
        private string codigo = string.Empty;
        private string nombre = string.Empty;
        private decimal precioUnitario;
        private bool tieneImpuesto;
        private int stock;

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

        public string Nombre
        {
            get => nombre;
            set { if (nombre != value) { nombre = value; OnPropertyChanged(); } }
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

        public int Stock
        {
            get => stock;
            set { if (stock != value) { stock = value; OnPropertyChanged(); } }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
