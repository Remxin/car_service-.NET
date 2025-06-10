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
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "update_service_order"
            });
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
    
    [HttpPatch("{taskId:int}")]
    public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UpdateServiceTaskRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServicePartResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "update_service_order"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.UpdateServiceTaskAsync(new UpdateServiceTaskRequest {
                Description = body.Description,
                ServiceTaskId = taskId,
                LaborCost = body.LaborCost
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service part");
            return StatusCode(500, new AddServicePartResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServicePartResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "update_service_order"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteServiceTaskAsync(new DeleteServiceTaskRequest { ServiceTaskId = taskId });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service part");
            return StatusCode(500, new AddServicePartResponse { Success = false, Message = "Internal server error" });
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
