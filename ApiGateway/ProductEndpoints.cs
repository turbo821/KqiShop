using ApiGateway.Dtos;
using Grpc.Core;
using ProductService.Client;

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
                 .Produces<GetProductResponse>()   
                 .ProducesProblem(404)
                 .WithName("GetProduct");

            group.MapPost("/", AddProduct)
                 .Produces<AddProductResponseDto>(201)
                 .ProducesProblem(400)
                 .WithName("AddProduct");

            group.MapPut("/{id}/stock", UpdateProductStock)
                 .Produces<UpdateProductStockResponse>()
                 .ProducesProblem(400)
                 .WithName("UpdateProductStock");
        }

        private static async Task<IResult> GetProduct(
            int id,
            ProductGrpc.ProductGrpcClient client,
            ILogger<Program> logger)
        {
            try
            {
                var response = await client.GetProductAsync(new GetProductRequest { ProductId = id });
                return TypedResults.Ok(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                logger.LogWarning("Product {ProductId} not found", id);
                return TypedResults.NotFound(ex.Status.Detail);
            }
        }

        private static async Task<IResult> AddProduct(
            AddProductRequest request,
            ProductGrpc.ProductGrpcClient client)
        {
            var response = await client.AddProductAsync(request);

            var dto = new AddProductResponseDto(response.Id, response.Success);
            return response.Success
                ? TypedResults.Created(nameof(GetProduct), dto)
                : TypedResults.BadRequest("Failed to add product");
        }

        private static async Task<IResult> UpdateProductStock(
            int id,
            UpdateProductStockRequest request,
            ProductGrpc.ProductGrpcClient client)
        {
            var response = await client.UpdateProductStockAsync(new UpdateProductStockRequest
            {
                ProductId = id,
                Stock = request.Stock
            });

            return response.Success
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest("Failed to update stock");
        }
    }
}
