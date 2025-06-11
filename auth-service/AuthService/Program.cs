using AuthService;
using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;

var config = new EnvConfig();

var builder = WebApplication.CreateBuilder(args);
var portStr = config.AuthServicePort ?? "5005";
if (!int.TryParse(portStr, out var port)) {
    port = 5005;
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


builder.Services.AddSingleton<JwtTokenService>(new JwtTokenService(config.JwtSecretKey));
builder.Services.AddSingleton<PasswordService>(new PasswordService());
builder.Services.AddSingleton<ReportEventPublisher>();
builder.Services.AddSingleton<EmailServiceEventPublisher>();

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Database.CanConnectAsync())
        throw new Exception("❌ Cannot connect to database");
}


app.MapGrpcReflectionService();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<AuthServiceImpl>();

app.Run();