using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;

namespace FileUploadService.Controllers;
[ApiController]
[Route("v1/car-image")]
public class CarImageController: ControllerBase {
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    private readonly ILogger<CarImageController> _logger;
    private readonly AuthService.AuthServiceClient _authClient;
    private readonly string _uploadAction = "create_service_order";
    
    public CarImageController(ILogger<CarImageController> logger, AuthService.AuthServiceClient authClient) {
        if (!Directory.Exists(_uploadPath)) {
            Directory.CreateDirectory(_uploadPath);
        }
        _logger = logger;
        _authClient = authClient;
    }

    /// <summary>
    /// Uploads car image
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing");
            return Unauthorized("Missing or invalid token");
        }
        
        var validationResult = await _authClient.VerifyActionAsync(new VerifyActionRequest {
            Action = _uploadAction,
            Token = token
        });
        
        if (validationResult == null) {
            _logger.LogWarning("Authorization service empty response on upload");
            return StatusCode(StatusCodes.Status500InternalServerError, "Authorization service empty response");
        }

        if (!validationResult.Allowed) {
            _logger.LogWarning("Token validation failed");
            return Unauthorized("Invalid token");
        }
        
        if (file == null || file.Length == 0) {
            _logger.LogWarning("File is not provided or empty");
            return BadRequest("No file provided");
        }

        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_uploadPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { FileName = fileName });
    }

    /// <summary>
    /// Gets car image
    /// </summary>
    [HttpGet("{fileName}")]
    public IActionResult GetImage(string fileName) {
        var filePath = Path.Combine(_uploadPath, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found");

        var contentType = "image/jpeg";
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, contentType, fileName);
    }
    
    /// <summary>
    /// Deletes a car image
    /// </summary>
    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteImage(string fileName) {
        var token = GetAccessToken();
        if (string.IsNullOrEmpty(token)) {
            _logger.LogWarning("Authorization header missing for delete");
            return Unauthorized("Missing or invalid token");
        }

        var validationResult = await _authClient.VerifyActionAsync(new VerifyActionRequest {
            Action = _uploadAction,
            Token = token
        });

        if (validationResult == null) {
            _logger.LogWarning("Authorization service empty response on delete");
            return StatusCode(StatusCodes.Status500InternalServerError, "Authorization service empty response");
        }

        if (!validationResult.Allowed) {
            _logger.LogWarning("Token validation failed on delete");
            return Unauthorized("Invalid token");
        }

        var filePath = Path.Combine(_uploadPath, fileName);
        if (!System.IO.File.Exists(filePath)) {
            _logger.LogWarning("File to delete not found: {FileName}", fileName);
            return NotFound("File not found");
        }

        try {
            System.IO.File.Delete(filePath);
            _logger.LogInformation("Deleted image file: {FileName}", fileName);
            return NoContent();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete the file");
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