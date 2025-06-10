using Gateway.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Models;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/users")] 
public class AuthController (
        AuthService.AuthServiceClient authServiceClient,
        WorkshopService.WorkshopServiceClient workshopServiceClient,
        ReportService.ReportServiceClient reportServiceClient,
        ILogger<AuthController> logger
    ): ControllerBase {
    
    private readonly ILogger<AuthController> _logger = logger;
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ReportService.ReportServiceClient _reportServiceClient = reportServiceClient;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersWithRolesAsync() {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetUsersWithRolesResponse {
                UsersWithRoles = {  }
            });
        }

        try {
            var users = await _authServiceClient.GetUsersWithRolesAsync(new GetUsersWithRolesRequest {
                Token = token
            });

            return Ok(new GetUsersWithRolesResponse {
                UsersWithRoles = { users.UsersWithRoles }
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to fetch users");
            return StatusCode(500, new GetUsersWithRolesResponse {
                UsersWithRoles = {  }
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestBody body) {
        try {
            var res = await _authServiceClient.LoginAsync(new LoginRequest {
                Email = body.Email,
                Password = body.Password
            });
            if (!res.Success) {
                _logger.LogWarning("Failed to login");
                return NotFound(new LoginResponse {
                    Message = res.Message,
                });
            }

            return Ok(res);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to fetch users ");
            return StatusCode(500, new LoginResponse() {
                Message = "Failed to login - local server error"
            });
        }
        
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new RegisterResponse {
                Success = false,
                Message = "No token provided"
            });
        }

        try {
            var perm = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "manage_users"
            });
            _logger.LogInformation(body.Email);
            if (!perm.Allowed) {
              
                _logger.LogWarning($"User has no permissions {perm.Message}, {token}");
                return Unauthorized(
                    new RegisterResponse {
                        Success = false,
                        Message = perm.Message,
                    });
            }

            var res = await _authServiceClient.RegisterAsync(body);

            return res.Success ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to fetch users ");
            return StatusCode(500, new LoginResponse() {
                Message = "Failed to login - local server error"
            });
        }
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleRequestBody body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddRoleResponse {
                Success = false,
                Message = "No token provided",
            });
        }

        try {
            var res = await _authServiceClient.AddRoleAsync(new AddRoleRequest {
                Token = token,
                RoleId = body.RoleId,
                UserId = body.UserId
            });

            if (!res.Success) {
                _logger.LogWarning("Failed to add role");
                return BadRequest(new AddRoleResponse {
                    Success = false,
                    Message = res.Message,
                });
            }

            return Ok(res);

        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to add role");
            return StatusCode(500, new AddRoleResponse {
                Success = false,
                Message = "Internal server error",
            });
        }
    }
    
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequestBody body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new RemoveRoleResponse() {
                Success = false,
                Message = "No token provided",
            });
        }

        try {
            var res = await _authServiceClient.RemoveRoleAsync(new RemoveRoleRequest {
                Token = token,
                RoleId = body.RoleId,
                UserId = body.UserId
            });

            if (!res.Success) {
                _logger.LogWarning("Failed to add role");
                return BadRequest(new RemoveRoleResponse() {
                    Success = false,
                    Message = res.Message,
                });
            }

            return Ok(res);

        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to add role");
            return StatusCode(500, new RemoveRoleResponse {
                Success = false,
                Message = "Internal server error",
            });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyUser() {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new VerifyUserResponse{
                IsValid = false,
                Message = "No token provided",
            });
        }

        try {
            var res = await _authServiceClient.VerifyUserAsync(new VerifyUserRequest {
                Token = token,
            });
            if (!res.IsValid) {
                return Unauthorized(res);
            }
            return Ok(res);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to verify user");
            return StatusCode(500, new VerifyUserResponse {
                IsValid = false,
                Message = "Internal server error",
            });
        }
    }
    
    private string? GetAccessToken() {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ")) {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        _logger.LogWarning("No authorization header found");
        return null;
    }
    
}