using OrderService.Application.Dtos;

namespace OrderService.Application.Interfaces
{
    public interface IProductService
    {
        Task<GetProductResponseDto> GetProductAsync(int productId);
        Task<bool> ReserveStockAsync(int productId, int quantity);
    }
}
