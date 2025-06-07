using Gateway.Api;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var config = new EnvConfig();
var builder = WebApplication.CreateBuilder(args);
var portStr = config.GatewayPort ?? "5010";
if (!int.TryParse(portStr, out var port)) {
    port = 5010;
}

builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddGrpcClient<Shared.Grpc.Services.AuthService.AuthServiceClient>(options => {
    options.Address = new Uri(config.AuthServiceUrl);
});

builder.Services.AddGrpcClient<Shared.Grpc.Services.WorkshopService.WorkshopServiceClient>(options => {
    options.Address = new Uri(config.WorkshopServiceUrl);
});

builder.Services.AddGrpcClient<Shared.Grpc.Services.ReportService.ReportServiceClient>(options => {
    options.Address = new Uri(config.ReportServiceUrl);
});


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(port);
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();