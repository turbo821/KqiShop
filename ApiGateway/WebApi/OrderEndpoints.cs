using Grpc.Core;
using ApiGateway.Application.Dtos.Order;
using Application.Interfaces;

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
            IOrderService orderService,
            ILogger<Program> logger)
        {
            try
            {
                var response = await orderService.CreateOrderAsync(request);

                return TypedResults.Created(nameof(GetOrder), response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                logger.LogWarning("Order creation failed: {Detail}", ex.Status.Detail);
                return TypedResults.BadRequest(ex.Status.Detail);
            }
        }

        private static async Task<IResult> GetOrder(
            int id,
            IOrderService orderService)
        {
            try
            {
                var response = await orderService.GetOrderAsync(id);

                return TypedResults.Ok(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return TypedResults.NotFound($"Order with id {id} not found");
            }
        }
    }
}
