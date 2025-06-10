using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/vehicle-parts")]
public class VehiclePartController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<VehiclePartController> logger
) : ControllerBase
{
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ILogger<VehiclePartController> _logger = logger;
    
    [HttpPost]
    public async Task<IActionResult> AddVehiclePart([FromBody] AddVehiclePartRequest body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new SearchVehiclePartsResponse { Success = false, Message = "No token provided" });

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "create_service_order",
            });
            if (!authres.Allowed) return Unauthorized(authres);
            var response = await _workshopServiceClient.AddVehiclePartAsync(body);

            return response.Success ? Ok(response) : BadRequest((object)response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching service parts");
            return StatusCode(500, new SearchVehiclePartsResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetServicePart(int id)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new GetVehiclePartResponse { Success = false, Message = "No token provided" });

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "list_vehicle_parts",
            });
            if (!authres.Allowed) return Unauthorized(authres);

            var response = await _workshopServiceClient.GetVehiclePartAsync(new GetVehiclePartRequest { VehiclePartId = id });

            return response.Success ? Ok(response) : BadRequest((object)response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service part");
            return StatusCode(500, new GetVehiclePartResponse { Success = false, Message = "Internal server error" });
        }
    }

    // GET v1/service-parts/search?page=1&pageSize=10&name=...&partNumber=...
    [HttpGet]
    public async Task<IActionResult> SearchServiceParts([FromQuery] SearchVehiclePartsRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new SearchVehiclePartsResponse { Success = false, Message = "No token provided" });

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "list_vehicle_parts",
            });
            if (!authres.Allowed) return Unauthorized(authres);

            var request = new SearchVehiclePartsRequest
            {
                Page = body.Page,
                PageSize = body.PageSize,
                Name = body.Name ?? "",
                PartNumber = body.PartNumber ?? "",
                Description = body.Description ?? "",
            };

            var response = await _workshopServiceClient.SearchVehiclePartsAsync(request);

            return response.Success ? Ok(response) : BadRequest((object)response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching service parts");
            return StatusCode(500, new SearchVehiclePartsResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpPatch]
    public async Task<IActionResult> DeletePart([FromBody] DeleteVehiclePartRequest body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "update_service_order"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteVehiclePartAsync(body);

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{partId}")]
    public async Task<IActionResult> DeleteOrder(int partId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "update_service_order"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteVehiclePartAsync(new DeleteVehiclePartRequest() { VehiclePartId = partId } );

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
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