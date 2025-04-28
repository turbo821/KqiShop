
using ApiGateway.Application.Dtos.Product;
using ApiGateway.Application.Interfaces;
using ApiGateway.Infrastructure.Protos;

namespace ApiGateway.Infrastructure.Grpc
{
    public class ProductGrpcService : IProductService
    {
        private readonly ProductGrpc.ProductGrpcClient _client;

        public ProductGrpcService(ProductGrpc.ProductGrpcClient client)
        {
            _client = client;
        }

        public async Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request)
        {
            var response = await _client.AddProductAsync(new AddProductRequest
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock
            });

            var dto = new AddProductResponseDto(response.Id, response.Success);
            return dto;
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

        public async Task<UpdateProductStockResponseDto> ReserveStockAsync(int productId, int quantity)
        {
            var response = await _client.UpdateProductStockAsync(new UpdateProductStockRequest
            {
                ProductId = productId,
                Quantity = quantity
            });

            var dto = new UpdateProductStockResponseDto(response.Success, response.NewStock);
            return dto;
        }

        public async Task<CheckProductStockResponseDto> CheckStockAsync(int productId)
        {
            var response = await _client.CheckProductStockAsync(new GetProductRequest { ProductId = productId });

            var dto = new CheckProductStockResponseDto(response.Stock);
            return dto;
        }
    }
}
