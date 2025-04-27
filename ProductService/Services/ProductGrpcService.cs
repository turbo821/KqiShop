using Grpc.Core;
using ProductService.Interfaces;
using ProductService.Models;

namespace ProductService.Services
{
    public class ProductGrpcService : ProductGrpc.ProductGrpcBase
    {
        private readonly ILogger <ProductGrpcService> _logger;
        private readonly IProductRepository _repo;

        public ProductGrpcService(ILogger<ProductGrpcService> logger, IProductRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _repo.GetByIdAsync(request.ProductId);

            if (product == null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    $"Product with id {request.ProductId} not found"));
            }

            return new GetProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };
        }

        public override async Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock
            };

            var id = await _repo.AddAsync(product);

            if (id == null)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    $"Product don't added"));
            }

            return new AddProductResponse
            {
                Success = true,
                Id = id.Value
            };
        }

        public override async Task<UpdateProductStockResponse> UpdateProductStock(UpdateProductStockRequest request, ServerCallContext context)
        {
            var newStock = await _repo.UpdateStockAsync(request.ProductId, request.Stock);

            if (newStock == null)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    $"Product stock (id: {request.ProductId}) don't updated"));
            }

            return new UpdateProductStockResponse
            {
                Success = true,
                NewStock = newStock.Value
            };
        }
    }
}
