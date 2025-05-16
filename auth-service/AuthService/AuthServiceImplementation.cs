using System;
using System.Threading.Tasks;
using Grpc.Core;
using Shared.Grpc.Messages;
using Shared.Grpc.Services; // wygenerowane klasy i serwis z proto

namespace AuthService
{
    public class AuthServiceImpl : Shared.Grpc.Services.AuthService.AuthServiceBase

    {
    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var response = new LoginResponse
        {
            Success = true,
            Message = "Zalogowano pomyślnie",
            Token = "fake-jwt-token"
        };

        return Task.FromResult(response);
    }

    public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        // Przykładowa logika rejestracji
        var response = new RegisterResponse
        {
            Success = true,
            Message = "Zarejestrowano pomyślnie",
            UserId = Guid.NewGuid().ToString()
        };

        return Task.FromResult(response);
    }

    public override Task<VerifyUserResponse> VerifyUser(VerifyUserRequest request, ServerCallContext context)
    {
        // Przykładowa logika weryfikacji użytkownika
        var response = new VerifyUserResponse
        {
            IsValid = true,
            Message = "Użytkownik jest poprawny"
        };

        return Task.FromResult(response);
    }
    }
}