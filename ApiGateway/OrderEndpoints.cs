using Grpc.Core;
using ApiGateway.Dtos;
using OrderService.Client;

namespace ApiGateway
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/orders")
                          .WithTags("Orders")
                          .WithOpenApi();

            group.MapPost("/", CreateOrder)
                 .Produces<CreateOrderResponseDto>(201)
                 .ProducesProblem(400)
                 .WithName(nameof(CreateOrder));

            group.MapGet("/{id}", GetOrder)
                 .Produces<GetOrderResponseDto>()
                 .ProducesProblem(404)
                 .WithName(nameof(GetOrder));
        }

        private static async Task<IResult> CreateOrder(
            CreateOrderRequestDto request,
            OrderGrpc.OrderGrpcClient client,
            ILogger<Program> logger)
        {
            try
            {
                CreateOrderRequest order = new CreateOrderRequest 
                { 
                    OrderStatus = request.OrderStatus, 
                    ProductId = request.ProductId, 
                    Quantity = request.Quantity 
                };

                var response = await client.CreateOrderAsync(order);

                var dto = new CreateOrderResponseDto(response.OrderId, response.Success);
                return TypedResults.Created(nameof(GetOrder), dto);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                logger.LogWarning("Order creation failed: {Detail}", ex.Status.Detail);
                return TypedResults.BadRequest(ex.Status.Detail);
            }
        }

        private static async Task<IResult> GetOrder(
            int id,
            OrderGrpc.OrderGrpcClient client)
        {
            try
            {
                var response = await client.GetOrderAsync(new GetOrderRequest { OrderId = id });

                var dto = new GetOrderResponseDto(
                    response.OrderId, response.Status, response.ProductId, 
                    response.ProductName, response.ProductDescription, 
                    response.Price, response.Quantity,
                    response.CreatedAt.ToDateTime());

                return TypedResults.Ok(dto);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return TypedResults.NotFound($"Order with id {id} not found");
            }
        }
    }
}
