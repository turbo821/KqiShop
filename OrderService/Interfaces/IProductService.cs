using ProductService.Client;

namespace OrderService.Interfaces
{
    public interface IProductService
    {
        Task<GetProductResponse> GetProductAsync(int productId);
        Task<bool> ReserveStockAsync(int productId, int quantity);
    }
}
