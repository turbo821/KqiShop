namespace ApiGateway.Application.Dtos.Product
{
    public record GetProductResponseDto(
        int Id, string Name,
        string Description, double Price,
        int Stock);
}
