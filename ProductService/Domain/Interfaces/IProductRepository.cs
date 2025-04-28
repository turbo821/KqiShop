using ProductService.Domain.Entities;

namespace ProductService.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);

        Task<int?> AddAsync(Product product);

        Task<int?> UpdateStockAsync(int productId, int stock);
    }
}
