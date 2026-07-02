using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using StockPoint.WPF.Models;
using StockPoint.WPF.Services;

namespace StockPoint.WPF.ViewModels
{
    public partial class ConsultaOrdenViewModel : ObservableObject
    {
        private readonly OrdenService _ordenService;

        [ObservableProperty]
        private string _numeroBusqueda = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TieneOrden))]
        private OrdenConsulta? _orden;

        [ObservableProperty]
        private string _mensaje = string.Empty;

        [ObservableProperty]
        private bool _cargando;

        public bool TieneOrden => Orden != null;

        public ConsultaOrdenViewModel()
        {
            _ordenService = new OrdenService();
        }

        [RelayCommand]
        private async Task Buscar()
        {
            Mensaje = string.Empty;
            Orden = null;

            if (!int.TryParse(NumeroBusqueda?.Trim(), out int numero) || numero <= 0)
            {
                Mensaje = "Ingrese un número de orden válido.";
                return;
            }

            try
            {
                Cargando = true;
                var resultado = await _ordenService.ObtenerOrdenAsync(numero);

                if (resultado == null)
                    Mensaje = $"No se encontró la orden #{numero}.";
                else
                    Orden = resultado;
            }
            catch (System.Exception ex)
            {
                Mensaje = $"Error al consultar la orden: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }
    }
}
