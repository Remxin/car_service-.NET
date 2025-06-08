using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Models;
using Shared.Grpc.Services;

namespace Gateway.Api.Controllers;
[ApiController]
[Route("v1/reports")]
public class ReportController (
        AuthService.AuthServiceClient authServiceClient,
        ReportService.ReportServiceClient reportServiceClient,
        ILogger<ReportController> logger
    ) : ControllerBase {
    private readonly AuthService.AuthServiceClient _authServiceClient = authServiceClient;
    private readonly ReportService.ReportServiceClient _reportServiceClient = reportServiceClient;
    private readonly ILogger<ReportController> _logger = logger;
    
    [HttpGet]
    public async Task<IActionResult> GetReportsList([FromQuery] GetReportsListRequestBody requestBody) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetReportsListResponse());
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _reportServiceClient.GetReportsListAsync(new GetReportsListRequest{
                Page = requestBody.Page,
                PageSize = requestBody.PageSize,
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpGet("download-link/{reportId}")]
    public async Task<IActionResult> GetReportDownloadLink(string reportId) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetReportsListResponse());
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _reportServiceClient.GetReportDownloadLinkAsync(new GetReportDownloadLinkRequest{
                ReportId = reportId
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetReportsListResponse());
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _reportServiceClient.GenerateReportAsync(body);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpPost("send-email")]
    public async Task<IActionResult> GetReportDownloadLink([FromBody] SendEmailWithReportRequest body) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetReportsListResponse());
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _reportServiceClient.SendEmailWithReportAsync(body);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    [HttpGet("{reportId}/status")]
    
    public async Task<IActionResult> GetReportStatus(string reportId) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized(new GetReportsListResponse());
        }

        try {
            var authres = await _authServiceClient.VerifyActionAsync(new VerifyActionRequest { Token = token });
            if (!authres.Allowed)
                return Unauthorized(authres);

            var result = await _reportServiceClient.GetReportStatusAsync(new GetReportStatusRequest {
                ReportId = reportId
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(500, new AddOrderResponse { Success = false, Message = "Internal server error" });
        }
    }
    
    private string? GetAccessToken() {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        _logger.LogWarning("No authorization header found");
        return null;
    }
}