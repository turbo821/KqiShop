using OrderService.Interfaces;
using ProductService.Client;

namespace OrderService.Services
{
    public class ProductGrpcClient : IProductService
    {
        private readonly ProductGrpc.ProductGrpcClient _client;

        public ProductGrpcClient(ProductGrpc.ProductGrpcClient client)
        {
            _client = client;
        }

        public async Task<GetProductResponse> GetProductAsync(int productId)
        {
            var response = await _client.GetProductAsync(new GetProductRequest
            {
                ProductId = productId
            });

            return response;
        }

        public async Task<bool> ReserveStockAsync(int productId, int quantity)
        {
            var response = await _client.UpdateProductStockAsync(new UpdateProductStockRequest
            {
                ProductId = productId,
                Stock = -quantity
            });

            return response.Success;
        }
    }
}
