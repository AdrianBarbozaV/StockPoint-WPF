using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace StockPoint.WPF.Models
{
    public class Cliente : INotifyPropertyChanged
    {
        private int clienteId;
        private string nombre    = string.Empty;
        private string apellidos = string.Empty;
        private string cedula    = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        // La API devuelve "id" (no "clienteId")
        [JsonPropertyName("id")]
        public int ClienteId
        {
            get => clienteId;
            set { if (clienteId != value) { clienteId = value; OnPropertyChanged(); } }
        }

        public string Nombre
        {
            get => nombre;
            set { if (nombre != value) { nombre = value; OnPropertyChanged(); OnPropertyChanged(nameof(NombreCompleto)); } }
        }

        public string Apellidos
        {
            get => apellidos;
            set { if (apellidos != value) { apellidos = value; OnPropertyChanged(); OnPropertyChanged(nameof(NombreCompleto)); } }
        }

        // La API devuelve "numeroIdentificacion" — se mapea a Cedula para compatibilidad con el ViewModel existente
        [JsonPropertyName("numeroIdentificacion")]
        public string Cedula
        {
            get => cedula;
            set { if (cedula != value) { cedula = value; OnPropertyChanged(); } }
        }

        // Campos adicionales de la API (ignorados en la UI pero necesarios para deserialización sin error)
        public string? Email     { get; set; }
        public string? Direccion { get; set; }
        public bool    Activo    { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();

        // Mostrado en el ComboBox de la orden
        public override string ToString() => $"{Cedula} - {NombreCompleto}";

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
