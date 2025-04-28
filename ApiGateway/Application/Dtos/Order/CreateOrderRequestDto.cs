
namespace ApiGateway.Application.Dtos.Order
{
    public record CreateOrderRequestDto(
        int ProductId,
        int Quantity,
        int OrderStatus
    );
}
