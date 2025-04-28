using ApiGateway.Application.Dtos.Product;
using ApiGateway.Application.Interfaces;
using Grpc.Core;

namespace ApiGateway
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/products")
                          .WithTags("Products")
                          .WithOpenApi();

            group.MapGet("/{id}", GetProduct)
                 .Produces<GetProductResponseDto>()   
                 .ProducesProblem(404)
                 .WithName(nameof(GetProduct));

            group.MapPost("/", AddProduct)
                 .Produces<AddProductResponseDto>(201)
                 .ProducesProblem(400)
                 .WithName(nameof(AddProduct));

            group.MapPut("/{id}/stock", UpdateProductStock)
                 .Produces<UpdateProductStockResponseDto>()
                 .ProducesProblem(400)
                 .WithName(nameof(UpdateProductStock));

            group.MapGet("/{id}/stock", CheckProductStock)
                 .Produces<CheckProductStockResponseDto>()
                 .ProducesProblem(400)
                 .WithName(nameof(CheckProductStock));
        }

        private static async Task<IResult> GetProduct(
            int id,
            IProductService productService,
            ILogger<Program> logger)
        {
            try
            {
                var response = await productService.GetProductAsync(id);
                return TypedResults.Ok(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                logger.LogWarning("Product {ProductId} not found", id);
                return TypedResults.NotFound(ex.Status.Detail);
            }
        }

        private static async Task<IResult> AddProduct(
            AddProductRequestDto request,
            IProductService productService)
        {
            var response = await productService.AddProductAsync(request);

            var dto = new AddProductResponseDto(response.Id, response.Success);
            return response.Success
                ? TypedResults.Created(nameof(GetProduct), dto)
                : TypedResults.BadRequest("Failed to add product");
        }

        private static async Task<IResult> UpdateProductStock(
            int id,
            UpdateProductStockRequestDto request,
            IProductService productService,
            ILogger<Program> logger)
        {
            try
            {
                var response = await productService.ReserveStockAsync(id, request.Quantity);

                return response.Success
                    ? TypedResults.Ok(response)
                    : TypedResults.BadRequest("Failed to update stock");
            }
            catch (RpcException ex)
            {
                logger.LogWarning("Product {ProductId} not found", id);
                return TypedResults.NotFound(ex.Status.Detail);
            }
        }

        private static async Task<IResult> CheckProductStock(
            int id,
            IProductService productService,
            ILogger<Program> logger)
        {
            try
            {
                var response = await productService.CheckStockAsync(id);

                return TypedResults.Ok(response);
            }
            catch (RpcException ex)
            {
                logger.LogWarning("Product {ProductId} not found", id);
                return TypedResults.NotFound(ex.Status.Detail);
            }
        }
    }
}
