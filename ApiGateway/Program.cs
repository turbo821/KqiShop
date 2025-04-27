using ApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();
