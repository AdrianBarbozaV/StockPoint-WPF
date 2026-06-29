using StockPoint.WPF.Models;

namespace StockPoint.WPF.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
    }
}
