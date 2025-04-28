using ApiGateway.Application.Dtos.Order;
using ApiGateway.Infrastructure.Protos;
using Application.Interfaces;

namespace ApiGateway.Infrastructure.Grpc
{
    public class OrderGrpcService : IOrderService
    {
        private readonly OrderGrpc.OrderGrpcClient _client;

        public OrderGrpcService(OrderGrpc.OrderGrpcClient client)
        {
            _client = client;
        }

        public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            CreateOrderRequest order = new CreateOrderRequest
            {
                OrderStatus = request.OrderStatus,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            var response = await _client.CreateOrderAsync(order);

            var dto = new CreateOrderResponseDto(response.OrderId, response.Success);
            return dto;
        }

        public async Task<GetOrderResponseDto> GetOrderAsync(int id)
        {
            var response = await _client.GetOrderAsync(new GetOrderRequest { OrderId = id });

            var dto = new GetOrderResponseDto(
                response.OrderId, response.Status, response.ProductId,
                response.ProductName, response.ProductDescription,
                response.Price, response.Quantity,
                response.CreatedAt.ToDateTime());

            return dto;
        }
    }
}
