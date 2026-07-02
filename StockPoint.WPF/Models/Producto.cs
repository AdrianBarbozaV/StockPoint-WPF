using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockPoint.WPF.Models
{
    public class Producto : INotifyPropertyChanged
    {
        private int _productId;
        private string _codigoBarra = string.Empty;
        private string _nombreEtiqueta = string.Empty;
        private string _descripcion = string.Empty;
        private decimal _precioNeto;
        private decimal _precioMinimo;
        private bool _tieneImpuesto;
        private decimal _impuestoValorAgregado;
        private int _existenciaEnStock;
        private int _existenciaLimiteParaAlerta;
        private bool _puedeVenderse = true;
        private bool _puedeComprarse = true;

        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); }
        }

        public string CodigoBarra
        {
            get => _codigoBarra;
            set { _codigoBarra = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string NombreEtiqueta
        {
            get => _nombreEtiqueta;
            set { _nombreEtiqueta = value; OnPropertyChanged(); }
        }

        public string Descripcion
        {
            get => _descripcion;
            set { _descripcion = value; OnPropertyChanged(); }
        }

        public decimal PrecioNeto
        {
            get => _precioNeto;
            set { _precioNeto = value; OnPropertyChanged(); }
        }

        public decimal PrecioMinimo
        {
            get => _precioMinimo;
            set { _precioMinimo = value; OnPropertyChanged(); }
        }

        public bool TieneImpuesto
        {
            get => _tieneImpuesto;
            set { _tieneImpuesto = value; OnPropertyChanged(); }
        }

        public decimal ImpuestoValorAgregado
        {
            get => _impuestoValorAgregado;
            set { _impuestoValorAgregado = value; OnPropertyChanged(); }
        }

        public int ExistenciaEnStock
        {
            get => _existenciaEnStock;
            set { _existenciaEnStock = value; OnPropertyChanged(); OnPropertyChanged(nameof(EstadoStock)); }
        }

        public int ExistenciaLimiteParaAlerta
        {
            get => _existenciaLimiteParaAlerta;
            set { _existenciaLimiteParaAlerta = value; OnPropertyChanged(); OnPropertyChanged(nameof(EstadoStock)); }
        }

        public bool PuedeVenderse
        {
            get => _puedeVenderse;
            set { _puedeVenderse = value; OnPropertyChanged(); }
        }

        public bool PuedeComprarse
        {
            get => _puedeComprarse;
            set { _puedeComprarse = value; OnPropertyChanged(); }
        }

        public string PrecioFormateado => $"₡{PrecioNeto:N0}";

        // "ok" | "alerta" | "critico" — usado por el semáforo del DataGrid
        public string EstadoStock
        {
            get
            {
                if (ExistenciaEnStock == 0) return "critico";
                if (ExistenciaLimiteParaAlerta > 0 && ExistenciaEnStock <= ExistenciaLimiteParaAlerta) return "alerta";
                return "ok";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
