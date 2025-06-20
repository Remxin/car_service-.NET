using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("v1/service-comments")]
public class ServiceCommentController(
    AuthService.AuthServiceClient authServiceClient,
    WorkshopService.WorkshopServiceClient workshopServiceClient,
    ILogger<ServiceCommentController> logger
) : ControllerBase
{
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly WorkshopService.WorkshopServiceClient _workshopServiceClient = workshopServiceClient;
    private readonly ILogger<ServiceCommentController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> AddServiceComment([FromBody] AddServiceCommentRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new AddServiceCommentResponse { Success = false, Message = "No token provided" });
        }

        try
        {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "add_service_comment",
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var response = await _workshopServiceClient.AddServiceCommentAsync(new AddServiceCommentRequest
            {
                OrderId = body.OrderId,
                Content = body.Content
            });

            return response.Success ? Ok(response) : BadRequest((object)response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service comment");
            return StatusCode(500, new AddServiceCommentResponse { Success = false, Message = "Internal server error" });
        } 
    }
    
    [HttpPatch("{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateServiceCommentRequestBody body)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest {
                Token = token,
                Action = "add_service_comment"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.UpdateServiceCommentAsync(new UpdateServiceCommentRequest {
                Content = body.Content,
                ServiceCommentId = commentId
            });

            return result.Success ? Ok(result) : BadRequest((object)result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, new GetOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteOrderComment(int commentId)
    {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetOrderResponse { Success = false, Message = "No token provided" });
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { 
                Token = token,
                Action = "add_service_comment"
            });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _workshopServiceClient.DeleteServiceCommentAsync(new DeleteServiceCommentRequest { ServiceCommentId = commentId } );

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