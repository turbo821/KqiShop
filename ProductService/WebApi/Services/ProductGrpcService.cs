using Grpc.Core;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.WebApi.Protos;

namespace ProductService.WebApi.Services
{
    public class ProductGrpcService : ProductGrpc.ProductGrpcBase
    {
        private readonly ILogger<ProductGrpcService> _logger;
        private readonly IProductRepository _repo;

        public ProductGrpcService(ILogger<ProductGrpcService> logger, IProductRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting product with ID {ProductId}", request.ProductId);

            try
            {
                var product = await _repo.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", request.ProductId);
                    throw new RpcException(new Status(
                        StatusCode.NotFound,
                        $"Product with id {request.ProductId} not found"));
                }

                _logger.LogDebug("Retrieved product: {@Product}", product);
                return new GetProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID {ProductId}", request.ProductId);
                throw;
            }
        }

        public override async Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Adding new product: {ProductName}", request.Name);

            try
            {
                if (request.Stock < 0)
                {
                    _logger.LogWarning("Attempt to add product with negative stock: {Stock}", request.Stock);
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        "Stock cannot be negative"));
                }

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Stock = request.Stock
                };

                _logger.LogDebug("Creating product: {@Product}", product);
                var id = await _repo.AddAsync(product);

                if (id == null)
                {
                    _logger.LogError("Failed to add product: {@Product}", product);
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        "Failed to add product"));
                }

                _logger.LogInformation("Product added successfully with ID {ProductId}", id.Value);
                return new AddProductResponse
                {
                    Success = true,
                    Id = id.Value
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Internal server error"));
            }
        }

        public override async Task<UpdateProductStockResponse> UpdateProductStock(UpdateProductStockRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating stock for product ID {ProductId}. New quantity: {Quantity}",
                request.ProductId, request.Quantity);

            try
            {
                var newStock = await _repo.UpdateStockAsync(request.ProductId, request.Quantity);

                if (newStock == null)
                {
                    _logger.LogWarning("Failed to update stock for product ID {ProductId}", request.ProductId);
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        $"Product stock (id: {request.ProductId}) not updated"));
                }

                _logger.LogInformation("Stock updated successfully for product ID {ProductId}. New stock: {NewStock}",
                    request.ProductId, newStock.Value);

                return new UpdateProductStockResponse
                {
                    Success = true,
                    NewStock = newStock.Value
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product ID {ProductId}", request.ProductId);
                throw;
            }
        }

        public override async Task<CheckProductStockResponse> CheckProductStock(GetProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Check stock for product ID {ProductId}.",
                    request.ProductId);
            try
            {
                var stock = await _repo.GetStockAsync(request.ProductId);

                return new CheckProductStockResponse
                {
                    Stock = stock.Value,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checked stock for product ID {ProductId}", request.ProductId);
                throw;
            }
        }
    }
}
