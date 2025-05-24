using AuthService;
using WorkshopService;
using WorkshopService.Data;
using WorkshopService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var config = new EnvConfig();

var builder = WebApplication.CreateBuilder(args);
var portStr = config.WorkshopServicePort ?? "5006";
if (!int.TryParse(portStr, out var port)) {
    port = 5006;
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

builder.Services.AddGrpcClient<Shared.Grpc.Services.AuthService.AuthServiceClient>(options => {
    options.Address = new Uri(config.AuthServiceUrl);
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(config.DbConnectionString));


builder.Services.AddSingleton<IConnection>(sp => {
    var factory = new ConnectionFactory {
        HostName = config.RabbitMQHost,
        Port = int.Parse(config.RabbitMQPort ?? "5672"),
        UserName = config.RabbitMQUserName,
        Password = config.RabbitMQPassword,
        VirtualHost = config.RabbitMQVirtualHost
    };
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("RabbitMQConnection");

    try {
        var connection = factory.CreateConnection();
        logger.LogInformation("✅ Connected to RabbitMQ");
        return connection;
    }
    catch (Exception ex) {
        logger.LogError(ex, "❌ RabbitMQ connection failed");
        throw;
    }
});
builder.Services.AddSingleton<IWorkshopEventPublisher, WorkshopEventPublisher>();


// Dodaj usługi do kontenera DI
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Dodaj Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Database.CanConnectAsync())
        throw new Exception("❌ Cannot connect to database");
    logger.LogInformation("Connected to Database");
    
    scope.ServiceProvider.GetRequiredService<IConnection>();
}



// Konfiguracja HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
app.MapGrpcReflectionService();
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapowanie kontrolerów API
app.MapControllers();

// Mapowanie serwisów gRPC
app.MapGrpcService<WorkshopServiceImplementation>();

app.Run();