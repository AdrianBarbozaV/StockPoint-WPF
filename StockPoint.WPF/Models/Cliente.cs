using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockPoint.WPF.Models
{
    public class Cliente : INotifyPropertyChanged
    {
        private int clienteId;
        private string nombre = string.Empty;
        private string cedula = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ClienteId
        {
            get => clienteId;
            set { if (clienteId != value) { clienteId = value; OnPropertyChanged(); } }
        }

        public string Nombre
        {
            get => nombre;
            set { if (nombre != value) { nombre = value; OnPropertyChanged(); } }
        }

        public string Cedula
        {
            get => cedula;
            set { if (cedula != value) { cedula = value; OnPropertyChanged(); } }
        }

        public override string ToString() => $"{Cedula} - {Nombre}";

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
