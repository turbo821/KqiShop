using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderGrpcService : OrderGrpc.OrderGrpcBase
    {
        private readonly ILogger<OrderGrpcService> _logger;
        private readonly IProductService _productService;
        private readonly IOrderRepository _orderRepository;

        public OrderGrpcService(
            ILogger<OrderGrpcService> logger,
            IProductService productService,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _productService = productService;
            _orderRepository = orderRepository;
        }

        public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                var product = await _productService.GetProductAsync(request.ProductId);

                if (product.Stock < request.Quantity)
                {
                    throw new RpcException(new Status(
                        StatusCode.FailedPrecondition,
                        $"Not enough stock for product {request.ProductId}"));
                }

                var order = new Order
                {
                    CreatedAt = DateTime.UtcNow,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Status = OrderStatus.Created
                };

                var orderId = await _orderRepository.AddAsync(order);

                var reserveSuccess = await _productService.ReserveStockAsync(
                    request.ProductId,
                    request.Quantity);

                if (!reserveSuccess)
                {
                    await _orderRepository.UpdateStatusAsync(orderId, OrderStatus.Failed);
                    throw new RpcException(new Status(
                        StatusCode.Aborted, 
                        "Failed to reserve product"));
                }

                return new CreateOrderResponse
                {
                    OrderId = orderId,
                    Success = true
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    $"Order with id {request.OrderId} not found"));
            }

            var product = await _productService.GetProductAsync(order.ProductId);

            return new GetOrderResponse
            {
                OrderId = order.Id,
                ProductId = order.ProductId,
                ProductName = product.Name,
                ProductDescription = product.Description,
                Price = product.Price,
                Quantity = order.Quantity,
                Status = ((int)order.Status),
                CreatedAt = Timestamp.FromDateTime(order.CreatedAt)
            };
        }
    }
}
