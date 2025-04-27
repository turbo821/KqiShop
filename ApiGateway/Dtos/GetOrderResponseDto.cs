namespace ApiGateway.Dtos
{
    public record GetOrderResponseDto(
        int OrderId, 
        int Status, 
        int ProductId, 
        string ProductName, 
        string ProductDescription, 
        double Price, 
        int Quantity,
        DateTime CreatedAt
    );
}
