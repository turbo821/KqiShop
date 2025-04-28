using ApiGateway.Application.Dtos.Order;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<GetOrderResponseDto> GetOrderAsync(int id);

        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
    }
}
