namespace OrderService.Application.Dtos
{
    public record GetProductResponseDto(
        int Id,
        string Name,
        string Description,
        double Price,
        int Stock
    );
}