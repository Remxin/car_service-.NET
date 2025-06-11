using System.Security.Claims;
using AuthService.Data;
using AuthService.Entities;
using AuthService.Mappers;
using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Grpc.Messages;


namespace AuthService.Services;

public class AuthServiceImpl(JwtTokenService tokenService, PasswordService passwordService, AppDbContext dbContext, ILogger<AuthServiceImpl> logger, EmailServiceEventPublisher emailServiceEventPublisher, ReportEventPublisher reportEventPublisher) : Shared.Grpc.Services.AuthService.AuthServiceBase {
    private readonly JwtTokenService _tokenService = tokenService;
    private readonly PasswordService _passwordService = passwordService;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<AuthServiceImpl> _logger = logger;
    private readonly EmailServiceEventPublisher _emailServiceEventPublisher = emailServiceEventPublisher;
    private readonly ReportEventPublisher _reportEventPublisher = reportEventPublisher;

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
            response.User = user.ToProtoDto();
            _logger.LogInformation($"User {user.Email} logged in");

            return response;
        }
        catch {
            _logger.LogError("Login failed - db connection failed");
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
            var user = await _dbContext.Users.AddAsync(new UserEntity {
                    Name = request.FirstName + " " + request.LastName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                }
            );
            await _dbContext.SaveChangesAsync();
            response.Success = true;
            response.Message = "Ok";
            response.UserId = user.Entity.Id.ToString();
            
            _logger.LogInformation($"Created user {user.Entity.Id} : {user.Entity.Email}");
            _reportEventPublisher.PublishEvent("workshop.service.user.created", new {
                Id = user.Entity.Id,
                Name = user.Entity.Name,
                Email = user.Entity.Email,
                CreatedAt = user.Entity.CreatedAt
            }); 
            _emailServiceEventPublisher.PublishEvent("email.send.welcome_email", new {
                Receiver = new Receiver{
                    Email = user.Entity.Email,
                    Name = user.Entity.Name
                }
            });
            return response;
        }
        catch {
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
        
        var userIdClaim = tokenValid.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim)) {
            response.Message = "Invalid token claims";
            return response;
        }
        
        if (!int.TryParse(userIdClaim, out int userId)) {
            response.Message = "Invalid user ID format in token";
            return response;
        }

        var userWithRoles = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new {
                User = u,
                Roles = u.UserRoles
                    .Select(ur => ur.Role.Name)
                    .Distinct()
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (userWithRoles == null)
        {
            response.Message = "User not found";
            return response;
        }

        var user = userWithRoles.User;
        var permissionNames = userWithRoles.Roles;
        
        
        response.Message = "Ok";
        response.IsValid = true;
        response.User = user.ToProtoDto();
        response.Roles.AddRange(permissionNames);
        
        _logger.LogInformation($"User {user.Email} verified");
        return response;
    }

    public override async Task<VerifyActionResponse> VerifyAction(VerifyActionRequest request, ServerCallContext context) {
        var (isAuthorized, userId, message) = await Authorize(request.Token, request.Action);

        _logger.LogInformation($"User {userId} action {request.Action} verified");
        return new VerifyActionResponse {
            Allowed = isAuthorized,
            Message = message
        };
    }

    public override async Task<GetUsersWithRolesResponse> GetUsersWithRoles(GetUsersWithRolesRequest request, ServerCallContext context) {
        var response = new GetUsersWithRolesResponse();

        var users = await dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();

        var usersWithRoles = users.Select(u => new UserWithRoles {
            User = new UserDto {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CreatedAt = Timestamp.FromDateTime(u.CreatedAt.ToUniversalTime())
            },
            RoleName = {
                u.UserRoles.Select(ur => new Role {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description
                })
            }
        });

        response.UsersWithRoles.AddRange(usersWithRoles);
        return response;
    }
    public override async Task<AddRoleResponse> AddRole(AddRoleRequest request, ServerCallContext context) {
        var response = new AddRoleResponse {
            Success = false,
            Message = ""
        };

        var (isAuthorized, _, authMessage) = await Authorize(request.Token, "manage_users");
        if (!isAuthorized) {
            response.Message = authMessage;
            return response;
        }

        if (!int.TryParse(request.UserId, out var userId) || !int.TryParse(request.RoleId, out var roleId)) {
            response.Message = "Invalid user or role ID";
            return response;
        }

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);
        var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == roleId);
        if (!userExists || !roleExists) {
            response.Message = userExists ? "Role not found" : "User not found";
            return response;
        }

        var alreadyAssigned = await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (alreadyAssigned) {
            response.Message = "Role already assigned";
            return response;
        }

        var userRole = new UserRoleEntity { UserId = userId, RoleId = roleId };
        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync();
        
        var role = _dbContext.Roles.FirstOrDefault(r => r.Id == roleId);

        response.Success = true;
        response.Message = "Role added";
        _logger.LogInformation($"User {userId} added role {roleId}");
        _reportEventPublisher.PublishEvent("workshop.service.user.role_added", new {
            UserId = userId,
            Role = new RoleEntity {
                Name = role.Name,
                Description = role.Description,
                Id = role.Id,
            }
        });
        return response;
    }
    
    public override async Task<RemoveRoleResponse> RemoveRole(RemoveRoleRequest request, ServerCallContext context) {
        var response = new RemoveRoleResponse {
            Success = false,
            Message = ""
        };

        var (isAuthorized, _, authMessage) = await Authorize(request.Token, "manage_users");
        if (!isAuthorized) {
            response.Message = authMessage;
            return response;
        }

        if (!int.TryParse(request.UserId, out var userId) || !int.TryParse(request.RoleId, out var roleId)) {
            response.Message = "Invalid user or role ID";
            return response;
        }

        var userRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole == null) {
            response.Message = "Role not assigned to user";
            return response;
        }

        _dbContext.UserRoles.Remove(userRole);
        await _dbContext.SaveChangesAsync();

        var role = _dbContext.Roles.FirstOrDefault(r => r.Id == roleId);
        response.Success = true;
        response.Message = "Role removed";
        _logger.LogInformation($"User {userId} removed role {roleId}");
        _reportEventPublisher.PublishEvent("workshop.service.user.role_removed", new {
            UserId = userId,
            Role = role
        });
        return response;
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context) {
        var user = await _dbContext.Users.FindAsync(request.UserId);
        if (user == null) {
            return new GetUserResponse {
                User = null,
            };
        }

        return new GetUserResponse {
            User = user.ToProtoDto(),
        };
    }

    private async Task<(bool isAuthorized, int userId, string message)> Authorize(string token, string requiredPermission) {
        var tokenValid = _tokenService.VerifyToken(token);
        if (tokenValid == null)
            return (false, 0, "Invalid token");

        var userIdClaim = tokenValid.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return (false, 0, "Invalid user ID in token");

        var hasPermission = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_dbContext.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
            .Join(_dbContext.Permissions, pid => pid, p => p.Id, (pid, p) => p)
            .AnyAsync(p => p.Name == requiredPermission);

        return hasPermission
            ? (true, userId, "Authorized")
            : (false, userId, "Forbidden");
    }
}
