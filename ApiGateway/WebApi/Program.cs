using ApiGateway;
using ApiGateway.Application.Interfaces;
using ApiGateway.Infrastructure.Grpc;
using Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductGrpcService>();
builder.Services.AddScoped<IOrderService, OrderGrpcService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();
