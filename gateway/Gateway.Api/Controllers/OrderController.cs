using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;
[ApiController]
[Route("v1/orders")]
public class OrderController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<OrderController> logger
) : ControllerBase
{
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] AddOrderRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddOrderResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.AddOrderAsync(new AddOrderRequest
            {
                VehicleId = body.VehicleId,
                Status = body.Status,
                AssignedMechanicId = body.AssignedMechanicId
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.GetOrderAsync(new GetOrderRequest { ServiceOrderId = orderId });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchOrders([FromQuery] SearchOrdersRequestBody body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new SearchOrdersResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.SearchOrdersAsync(new SearchOrdersRequest {
                Page = body.Page,
                PageSize = body.PageSize,
                VehicleVin = body.VehicleVin ?? "",
                VehicleBrand = body.VehicleBrand ?? "",
                VehicleModel = body.VehicleModel ?? "",
                VehicleYear = body.VehicleYear ?? 0,
                CreatedAfter = body.CreatedAfter.HasValue
                    ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(body.CreatedAfter.Value.ToUniversalTime())
                    : null,
                CreatedBefore = body.CreatedBefore.HasValue
                    ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(body.CreatedBefore.Value.ToUniversalTime())
                    : null
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error searching orders");
            return StatusCode(500, new SearchOrdersResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.UpdateOrderAsync(body);

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteOrderAsync(new DeleteOrderRequest { ServiceOrderId = orderId } );

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
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            _logger.LogWarning("No authorization header found");
            return null;
        }
    }
