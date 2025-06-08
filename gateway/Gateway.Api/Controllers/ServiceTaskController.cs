using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;
using Google.Protobuf.WellKnownTypes;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/service-tasks")]
public class ServiceTaskController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<ServiceTaskController> logger
) : ControllerBase
{
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ILogger<ServiceTaskController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> AddServiceTask([FromBody] AddServiceTaskRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServiceTaskResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.AddServiceTaskAsync(new AddServiceTaskRequest
            {
                OrderId = body.OrderId,
                Description = body.Description,
                LaborCost = body.LaborCost
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service task");
            return StatusCode(500, new AddServiceTaskResponse { Success = false, Message = "Internal server error" });
        }
    }

    private string? GetAccessToken()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        _logger.LogWarning("No authorization header found");
        return null;
    }
}
