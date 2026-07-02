using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockPoint.WPF.Models
{
    public class OrdenDetalle : INotifyPropertyChanged
    {
        private int     productoId;
        private string  codigo                = string.Empty;
        private string  nombreProducto        = string.Empty;
        private decimal precioUnitario;
        private bool    tieneImpuesto;
        private decimal impuestoValorAgregado;
        private int     cantidad;

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
            set { if (precioUnitario != value) { precioUnitario = value; OnPropertyChanged(); NotifyCalculados(); } }
        }

        public bool TieneImpuesto
        {
            get => tieneImpuesto;
            set { if (tieneImpuesto != value) { tieneImpuesto = value; OnPropertyChanged(); NotifyCalculados(); } }
        }

        // Tasa real del producto (puede venir como fracción 0.13 o porcentaje 13).
        public decimal ImpuestoValorAgregado
        {
            get => impuestoValorAgregado;
            set { if (impuestoValorAgregado != value) { impuestoValorAgregado = value; OnPropertyChanged(); NotifyCalculados(); } }
        }

        public int Cantidad
        {
            get => cantidad;
            set
            {
                if (cantidad != value)
                {
                    cantidad = value;
                    OnPropertyChanged();
                    NotifyCalculados();
                }
            }
        }

        public decimal Subtotal => PrecioUnitario * Cantidad;

        public decimal ImpuestoLinea
        {
            get
            {
                if (!TieneImpuesto) return 0m;
                // Normaliza: si la tasa es > 1 es un porcentaje (ej. 13), si <= 1 ya es fracción (ej. 0.13)
                var tasa = ImpuestoValorAgregado > 1m ? ImpuestoValorAgregado / 100m : ImpuestoValorAgregado;
                return Math.Round(Subtotal * tasa, 2, MidpointRounding.AwayFromZero);
            }
        }

        private void NotifyCalculados()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(ImpuestoLinea));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
