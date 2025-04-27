using Grpc.Core;
using OrderService;

namespace OrderService.Services
{
    public class OrderGrpcService : OrderGrpc.OrderGrpcBase
    {
        private readonly ILogger<OrderGrpcService> _logger;
        public OrderGrpcService(ILogger<OrderGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            return base.CreateOrder(request, context);
        }

        public override Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            return base.GetOrder(request, context);
        }
    }
}
