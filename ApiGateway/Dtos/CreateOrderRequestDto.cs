
namespace ApiGateway.Dtos
{
    public record CreateOrderRequestDto(
        int ProductId,
        int Quantity,
        int OrderStatus
    );
}
