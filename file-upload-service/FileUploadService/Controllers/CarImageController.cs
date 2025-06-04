using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers;

[ApiController]
[Route("v1/car-image")]
public class CarImageController: ControllerBase {
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    private readonly ILogger<CarImageController> _logger;
    
    public CarImageController(ILogger<CarImageController> logger) {
        if (!Directory.Exists(_uploadPath)) {
            Directory.CreateDirectory(_uploadPath);
        }
        _logger = logger;
    }

    /// <summary>
    /// Uploads car image
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file) {
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
}