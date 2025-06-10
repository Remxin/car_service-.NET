using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/vehicles")] 
public class VehicleController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<AuthController> logger
) : ControllerBase {
    private readonly ILogger<AuthController> _logger = logger;
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;

    [HttpGet]
    public async Task<IActionResult> SearchVehicles([FromQuery] SearchVehiclesRequestBody body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new SearchVehiclesResponse{
                Success = false,
                Message = "No token provided",
            });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "list_vehicles"
            });
            if (!authres.Allowed) {
                return Unauthorized(new GetVehicleResponse {
                    Message = authres.Message,
                    Success = false,
                });
            }

            var vres = await _workshopServiceClient.SearchVehiclesAsync(new SearchVehiclesRequest {
                Page = body.Page,
                PageSize = body.PageSize,
            });
            
            if (!vres.Success) {
                return BadRequest((object)vres);
            }

            return Ok(vres);
            
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occured while getting vehicles");
            return StatusCode(500, new SearchVehiclesResponse {
                Message = "Internal server error",
                Success = false,
            });
        }
    }

    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> GetVehicle(int vehicleId) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new SearchVehiclesResponse {
                Success = false,
                Message = "No token provided",
            });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "list_vehicles"
            });
            if (!authres.Allowed) {
                return Unauthorized(authres);
            }

            var vres = await _workshopServiceClient.GetVehicleAsync(new GetVehicleRequest {
                VehicleId = vehicleId
            });
            
            if (!vres.Success) {
                return BadRequest((object)vres);
            }

            return Ok(vres);
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occured while getting vehicle");
            return StatusCode(500, new GetVehicleResponse {
                Success = false,
                Message = "Internal server error",
            });
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequestBody body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new SearchVehiclesResponse {
                Success = false,
                Message = "No token provided",
            });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "add_vehicles"
            });
            if (!authres.Allowed) {
                return Unauthorized(authres);
            }

            var vres = await _workshopServiceClient.AddVehicleAsync(new AddVehicleRequest {
                Brand = body.Brand,
                Model = body.Model,
                CarImageUrl = body.CarImageUrl,
                Year = body.Year,
                Vin = body.Vin,
            });
            
            if (!vres.Success) {
                return BadRequest((object)vres);
            }

            return Ok(vres);
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occured while getting vehicle");
            return StatusCode(500, new GetVehicleResponse {
                Success = false,
                Message = "Internal server error",
            });
        }
    }
    
    [HttpPatch("{vehicleId:int}")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] UpdateVehicleRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "add_vehicles"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.UpdateVehicleAsync(new UpdateVehicleRequest {
                VehicleId = vehicleId,
                Brand = body.Brand,
                Model = body.Model,
                CarImageUrl = body.CarImageUrl,
                Year = body.Year,
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> DeleteOrder(int vehicleId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "add_vehicles"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteVehicleAsync(new DeleteVehicleRequest() { VehicleId = vehicleId } );

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
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