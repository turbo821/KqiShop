namespace ApiGateway.Application.Dtos.Product
{
    public record AddProductRequestDto(
        string Name, string Description,
        double Price, int Stock);
}
