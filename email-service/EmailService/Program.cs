using EmailService;
using EmailService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RabbitMQ.Client;
using Serilog;

var config = new EnvConfig();
var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddSingleton<BlobStorageService>(sp =>{
    var logger = sp.GetRequiredService<ILogger<BlobStorageService>>();
    return new BlobStorageService(
        config.AzureBlobConnectionString,
        config.AzureBlobContainerName,
        logger
    );
});

builder.Services.AddSingleton<EmailSenderService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<EmailSenderService>>();
    var blobStorageService = sp.GetRequiredService<BlobStorageService>();
    return new EmailSenderService(
        senderEmail: config.EmailUser,
        appPassword: config.EmailPassword,
        blobStorageService: blobStorageService,
        logger
    );
});
builder.Services.AddSingleton<EmailServiceEventSubscriber>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application Started");


var subscriber = app.Services.GetRequiredService<EmailServiceEventSubscriber>();
subscriber.Start();

app.Run();