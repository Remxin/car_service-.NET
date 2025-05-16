
var builder = WebApplication.CreateBuilder(args);

// Dodaj usługi do kontenera DI
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Dodaj Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
app.MapGrpcService<AuthService.AuthServiceImpl>();

app.Run();