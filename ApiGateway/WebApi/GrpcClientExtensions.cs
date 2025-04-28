using ApiGateway.Infrastructure.Protos;

namespace ApiGateway
{
    public static class GrpcClientExtensions
    {
        public static IServiceCollection AddGrpcClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddGrpcClient<ProductGrpc.ProductGrpcClient>(options =>
            {
                options.Address = new Uri(configuration["ProductServiceUrl"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

            services.AddGrpcClient<OrderGrpc.OrderGrpcClient>(options =>
            {
                options.Address = new Uri(configuration["OrderServiceUrl"]!);
            });

            return services;
        }
    }
}
