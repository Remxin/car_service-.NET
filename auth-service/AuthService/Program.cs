using AuthService;
using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddSingleton<JwtTokenService>(new JwtTokenService(config.JwtSecretKey));
builder.Services.AddSingleton<PasswordService>(new PasswordService());

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