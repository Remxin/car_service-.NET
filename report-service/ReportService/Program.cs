using RabbitMQ.Client;
using ReportService;
using ReportService.Data;
using ReportService.Services;
using System.Linq.Expressions;
using MongoDB.Driver;
using ReportService.Entities;


var config = new EnvConfig();

var builder = WebApplication.CreateBuilder(args);
var portStr = config.ReportServicePort ?? "5007";
if (!int.TryParse(portStr, out var port)) {
    port = 5007;
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


// Add services to the container.
builder.Services.AddSingleton(new MongoDbContext(config.DbConnectionString));
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
builder.Services.AddSingleton<ReportServiceEventSubscriber>();
builder.Services.AddSingleton<IReportServiceEventPublisher, ReportServiceEventPublisher>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application Started");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    try
    {
        _ = db.Reports.CountDocuments(Builders<ReportEntity>.Filter.Empty);
        logger.LogInformation("✅ Connected to MongoDB");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ MongoDB connection failed");
        throw;
    }
    
    scope.ServiceProvider.GetRequiredService<IConnection>();
}

var subscriber = app.Services.GetRequiredService<ReportServiceEventSubscriber>();
subscriber.Start();

app.MapGrpcReflectionService();
app.MapGrpcService<ReportServiceImplementation>();

app.Run();