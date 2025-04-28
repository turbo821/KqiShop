using ApiGateway.Application.Dtos.Product;

namespace ApiGateway.Application.Interfaces
{
    public interface IProductService
    {
        Task<GetProductResponseDto> GetProductAsync(int id);

        Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request);

        Task<UpdateProductStockResponseDto> ReserveStockAsync(int productId, int quantity);

        Task<CheckProductStockResponseDto> CheckStockAsync(int productId);
    }
}
