using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Messages;


namespace AuthService.Services;

public class AuthServiceImpl(JwtTokenService tokenService, PasswordService passwordService, AppDbContext dbContext) : Shared.Grpc.Services.AuthService.AuthServiceBase {
    private readonly JwtTokenService _tokenService = tokenService;
    private readonly PasswordService _passwordService = passwordService;
    private readonly AppDbContext _dbContext = dbContext;

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context) {
        var response = new LoginResponse {
            Success = false,
            Message = "",
            Token = "",
        };

        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) {
                response.Message = "User not found";
                return response;
            }
            
            var validPasswd = _passwordService.VerifyPassword(user.PasswordHash, request.Password);
            if (!validPasswd) {
                response.Message = "Invalid password";
                return response;
            }

            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email);

            response.Message = "Ok";
            response.Success = true;
            response.Token = token;

            return response;
        }
        catch (Exception ex)
        {
            response.Message = "Database connection error";
            return response;
        }
    }


public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context) {
    var response = new RegisterResponse {
        Success = false,
        Message = "",
        UserId = ""
    };

    try {
        var passwordHash = _passwordService.HashPassword(request.Password);
        var user = await _dbContext.Users.AddAsync(new User {
                Name = request.FirstName + " " + request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                // CreatedAt = System.DateTime.Now,
            }
        );
        await _dbContext.SaveChangesAsync();
        response.Success = true;
        response.Message = "Ok";
        response.UserId = user.Entity.Id.ToString();
        
        return response;
    }
    catch (Exception ex) {
        response.Message = "Database connection error";
        return response;
    }
    
}

public override async Task<VerifyUserResponse> VerifyUser(VerifyUserRequest request, ServerCallContext context)
{
    var response = new VerifyUserResponse
    {
        IsValid = false,
        Message = ""
    };
    
    var tokenValid = _tokenService.VerifyToken(request.Token);
    if (tokenValid == null) {
        response.Message = "Invalid token";
        return response;
    }
    
    response.Message = "Ok";
    response.IsValid = true;
    return response;
}
}
