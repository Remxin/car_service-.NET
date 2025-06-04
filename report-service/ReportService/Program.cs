using RabbitMQ.Client;
using ReportService;
using ReportService.Data;
using ReportService.Services;
using System.Linq.Expressions;
using MongoDB.Driver;
using ReportService.Entities;
using QuestPDF.Infrastructure;
using Serilog;


var config = new EnvConfig();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
var portStr = config.ReportServicePort ?? "5007";
if (!int.TryParse(portStr, out var port)) {
    port = 5007;
}



builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddSingleton<ReportServiceEventPublisher>();
builder.Services.AddSingleton<BlobStorageService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<BlobStorageService>>();
    return new BlobStorageService(
        config.AzureBlobConnectionString,
        config.AzureBlobContainerName,
        logger
    );
});

builder.Services.AddSingleton<PdfGeneratorService>(provider =>
{
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedPdfs");
    var logger = provider.GetRequiredService<ILogger<PdfGeneratorService>>();
    return new PdfGeneratorService(folderPath, logger);
});
builder.Services.AddHostedService<OldReportCleanupService>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application Started");

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    try
    {
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