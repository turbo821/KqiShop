using Grpc.Core;
using ProductService;

namespace ProductService.Services
{
    public class ProductGrpcService : ProductGrpc.ProductGrpcBase
    {
        private readonly ILogger <ProductGrpcService> _logger;
        public ProductGrpcService(ILogger<ProductGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            return base.GetProduct(request, context);
        }

        public override Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            return base.AddProduct(request, context);
        }

        public override Task<UpdateProductStockResponse> UpdateProductStock(UpdateProductStockRequest request, ServerCallContext context)
        {
            return base.UpdateProductStock(request, context);
        }
    }
}
