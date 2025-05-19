using AuthService;
using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;

var config = new EnvConfig();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(config.DbConnectionString));

builder.Services.AddSingleton<JwtTokenService>(new JwtTokenService(config.JwtSecretKey));
builder.Services.AddSingleton<PasswordService>(new PasswordService());

// Dodaj usługi do kontenera DI
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Dodaj Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Database.CanConnectAsync())
        throw new Exception("❌ Cannot connect to database");
}



// Konfiguracja HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapowanie kontrolerów API
app.MapControllers();

// Mapowanie serwisów gRPC
app.MapGrpcService<AuthServiceImpl>();

app.Run();