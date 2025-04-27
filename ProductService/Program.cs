using ProductService.Services;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using ProductService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

string connection = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseNpgsql(connection));

builder.Services.AddScoped<IProductRepository, IProductRepository>();

var app = builder.Build();

app.MapGrpcService<ProductGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
