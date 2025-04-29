using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Application.Interfaces;
using OrderService.WebApi.Protos;

namespace OrderService.WebApi.Services
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
            _logger.LogInformation("Starting order creation for product {ProductId}, quantity {Quantity}",
                request.ProductId, request.Quantity);

            if (request.Quantity < 0)
            {
                _logger.LogWarning("Attempt to create order with negative quantity: {Quantity}", request.Quantity);
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Quantity cannot be negative"));
            }

            try
            {
                _logger.LogDebug("Checking product availability...");
                var product = await _productService.GetProductAsync(request.ProductId);

                if (product.Stock < request.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {AvailableStock}, Requested: {RequestedQuantity}",
                        request.ProductId, product.Stock, request.Quantity);

                    throw new RpcException(new Status(
                        StatusCode.FailedPrecondition,
                        $"Not enough stock for product {request.ProductId}"));
                }

                _logger.LogDebug("Creating order entity...");
                var order = new Order
                {
                    CreatedAt = DateTime.UtcNow,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Status = OrderStatus.Created
                };

                var orderId = await _orderRepository.AddAsync(order);
                _logger.LogInformation("Order {OrderId} created successfully", orderId);

                _logger.LogDebug("Reserving product stock...");
                var reserveSuccess = await _productService.ReserveStockAsync(
                    request.ProductId,
                    request.Quantity);

                if (!reserveSuccess)
                {
                    _logger.LogError("Failed to reserve stock for order {OrderId}", orderId);
                    await _orderRepository.UpdateStatusAsync(orderId, OrderStatus.Failed);

                    throw new RpcException(new Status(
                        StatusCode.Aborted,
                        "Failed to reserve product"));
                }

                _logger.LogInformation("Order {OrderId} completed successfully", orderId);
                return new CreateOrderResponse
                {
                    OrderId = orderId,
                    Success = true
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error creating order. Status: {StatusCode}, Detail: {Detail}",
                    ex.StatusCode, ex.Status.Detail);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error creating order");
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Internal server error"));
            }
        }

        public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Fetching order {OrderId}", request.OrderId);

            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", request.OrderId);
                    throw new RpcException(new Status(
                        StatusCode.NotFound,
                        $"Order with id {request.OrderId} not found"));
                }

                _logger.LogDebug("Fetching product details for order {OrderId}", order.Id);
                var product = await _productService.GetProductAsync(order.ProductId);

                _logger.LogDebug("Order details retrieved: {@Order}", new
                {
                    order.Id,
                    order.Status,
                    Product = product.Name
                });

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
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}. Status: {StatusCode}",
                    request.OrderId, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching order {OrderId}", request.OrderId);
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Internal server error"));
            }
        }
    }
}
