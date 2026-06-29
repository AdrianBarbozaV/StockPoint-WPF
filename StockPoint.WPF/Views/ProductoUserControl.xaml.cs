using StockPoint.WPF.Services;
using StockPoint.WPF.ViewModels;
using System.Windows.Controls;

namespace StockPoint.WPF.Views
{
    public partial class ProductoUserControl : UserControl
    {
        public ProductoUserControl()
        {
            InitializeComponent();

            var vm = new ProductoViewModel(new ProductoService());
            DataContext = vm;

            Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
