using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/service-parts")]
public class ServicePartController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<ServicePartController> logger
) : ControllerBase
{
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ILogger<ServicePartController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> AddServicePart([FromBody] AddServicePartRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServicePartResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.AddServicePartAsync(new AddServicePartRequest
            {
                OrderId = body.OrderId,
                VehiclePartId = body.VehiclePartId,
                Quantity = body.Quantity
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service part");
            return StatusCode(500, new AddServicePartResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateServicePart([FromBody] UpdateServicePartRequest body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServicePartResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.UpdateServicePartAsync(new UpdateServicePartRequest{
                OrderId = body.OrderId,
                VehiclePartId = body.VehiclePartId,
                Quantity = body.Quantity,
                ServicePartId = body.ServicePartId
                
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service part");
            return StatusCode(500, new AddServicePartResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{partId}")]
    public async Task<IActionResult> DeleteServicePart(int partId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServicePartResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteServicePartAsync(new DeleteServicePartRequest { ServicePartId = partId });

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
        return authHeader?.StartsWith("Bearer ") == true
            ? authHeader.Substring("Bearer ".Length).Trim()
            : null;
    }
}