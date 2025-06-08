using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileUploadService.Controllers;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Grpc.Messages;
using Shared.Grpc.Services;
using Xunit;

namespace FileUploadService.Tests;

public class CarImageControllerTests {
    private readonly Mock<ILogger<CarImageController>> _loggerMock = new();
    private readonly Mock<AuthService.AuthServiceClient> _authClientMock = new();

    public CarImageControllerTests() {
        // Możesz tutaj zostawić puste lub domyślne zachowanie
    }

    private static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response) where T : class {
        return new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { }
        );
    }

    private CarImageController CreateController() {
        return new CarImageController(_loggerMock.Object, _authClientMock.Object);
    }

    [Fact]
    public async Task UploadImage_ReturnsBadRequest_WhenFileIsNull() {
        // Arrange
        var response = new VerifyActionResponse { Allowed = true };
        _authClientMock
            .Setup(c => c.VerifyActionAsync(
                It.IsAny<VerifyActionRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncUnaryCall(response));

        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = "Bearer mock-token";

        // Act
        var result = await controller.UploadImage(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file provided", badRequestResult.Value);
    }

    [Fact]
    public async Task UploadImage_ReturnsUnauthorized_WhenTokenMissing() {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext() // brak headera "Authorization"
        };

        var result = await controller.UploadImage(Mock.Of<IFormFile>());

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UploadImage_ReturnsUnauthorized_WhenAuthFails() {
        var response = new VerifyActionResponse { Allowed = false };
        _authClientMock
            .Setup(c => c.VerifyActionAsync(
                It.IsAny<VerifyActionRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncUnaryCall(response));

        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = "Bearer mock-token";

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1);
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

        var result = await controller.UploadImage(fileMock.Object);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid token", unauthorizedResult.Value);
    }

    [Fact]
    public void GetImage_ReturnsNotFound_WhenFileMissing() {
        var controller = CreateController();
        var fakeFileName = "notexist.jpg";

        var result = controller.GetImage(fakeFileName);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("File not found", notFoundResult.Value);
    }
    
    [Fact]
    public async Task DeleteImage_ReturnsUnauthorized_WhenTokenMissing() {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };

        // brak headera Authorization
        var result = await controller.DeleteImage("somefile.jpg");

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Missing or invalid token", unauthorizedResult.Value);
    }

    [Fact]
    public async Task DeleteImage_ReturnsUnauthorized_WhenAuthFails() {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = "Bearer valid-token";

        _authClientMock
            .Setup(c => c.VerifyActionAsync(
                It.IsAny<VerifyActionRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncUnaryCall(new VerifyActionResponse { Allowed = false }));

        var result = await controller.DeleteImage("somefile.jpg");

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid token", unauthorizedResult.Value);
    }

    [Fact]
    public async Task DeleteImage_ReturnsNotFound_WhenFileDoesNotExist() {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = "Bearer valid-token";
        _authClientMock
            .Setup(c => c.VerifyActionAsync(
                It.IsAny<VerifyActionRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncUnaryCall(new VerifyActionResponse { Allowed = true }));

        var fakeFileName = "nonexistentfile.jpg";

        var result = await controller.DeleteImage(fakeFileName);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("File not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteImage_ReturnsNoContent_WhenFileDeleted() {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = "Bearer valid-token";
        _authClientMock
            .Setup(c => c.VerifyActionAsync(
                It.IsAny<VerifyActionRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncUnaryCall(new VerifyActionResponse { Allowed = true }));

        var fakeFileName = "testfile-to-delete.jpg";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fakeFileName);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(filePath, "dummy content");

        var result = await controller.DeleteImage(fakeFileName);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.False(File.Exists(filePath));
    }
    
    
}