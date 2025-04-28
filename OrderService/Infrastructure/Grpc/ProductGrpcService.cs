using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Protos;

namespace OrderService.Infrastructure.Services
{
    public class ProductGrpcService : IProductService
    {
        private readonly ProductGrpc.ProductGrpcClient _client;

        public ProductGrpcService(ProductGrpc.ProductGrpcClient client)
        {
            _client = client;
        }

        public async Task<GetProductResponseDto> GetProductAsync(int productId)
        {
            var response = await _client.GetProductAsync(new GetProductRequest
            {
                ProductId = productId
            });

            var dto = new GetProductResponseDto(
                response.Id, response.Name, 
                response.Description, response.Price, 
                response.Stock);

            return dto;
        }

        public async Task<bool> ReserveStockAsync(int productId, int quantity)
        {
            var response = await _client.UpdateProductStockAsync(new UpdateProductStockRequest
            {
                ProductId = productId,
                Quantity = -quantity
            });

            return response.Success;
        }
    }
}
