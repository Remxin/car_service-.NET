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
    public async Task<IActionResult> GetUsers() {
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
    
    // public async Task<>
    
    private string? GetAccessToken() {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ")) {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        _logger.LogWarning("No authorization header found");
        return null;
    }
    
}