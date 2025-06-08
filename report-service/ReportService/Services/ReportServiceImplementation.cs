using Google.Protobuf.WellKnownTypes;
using ReportService.Data;
using Grpc.Core;
using MongoDB.Driver;
using ReportService;
using ReportService.Entities;
using Shared.Grpc.Messages;
using Shared.Grpc.Models;

namespace ReportService.Services;


public class ReportServiceImplementation(
    BlobStorageService blobStorageService,
    MongoDbContext mongoDbContext,
    PdfGeneratorService pdfGeneratorService,
    ReportServiceEventPublisher reportServiceEventPublisher,
    WorkshopStatusPublisher workshopStatusPublisher,
    ILogger<ReportServiceImplementation> logger
    ) : Shared.Grpc.Services.ReportService.ReportServiceBase {
    private readonly BlobStorageService _blobStorageService = blobStorageService;
    private readonly MongoDbContext _mongoDbContext = mongoDbContext;
    private readonly PdfGeneratorService _pdfGeneratorService = pdfGeneratorService;
    private readonly ReportServiceEventPublisher _reportServiceEventPublisher = reportServiceEventPublisher;
    private readonly WorkshopStatusPublisher _workshopStatusPublisher = workshopStatusPublisher;
    private readonly ILogger<ReportServiceImplementation> _logger = logger;
    
    public override async Task<GetReportDownloadLinkResponse> GetReportDownloadLink(GetReportDownloadLinkRequest request,
        ServerCallContext context) {
        var response = new GetReportDownloadLinkResponse {
            ReportId = request.ReportId,
            DownloadUrl = "",
            ExpiresAt = null
        };
        var filter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, int.Parse(request.ReportId));
        var report = await _mongoDbContext.Reports.Find(filter).FirstOrDefaultAsync();
        if (report == null) {
            return response;
        }

        var blobName = Path.GetFileName(report.ReportUrl);
        var (sasUrl, expiresAt) = await _blobStorageService.GenerateDownloadLinkAsync(blobName);

        response.ReportId = request.ReportId;
        response.DownloadUrl = sasUrl;
        response.ExpiresAt = Timestamp.FromDateTime(expiresAt);
        
        return response;
    }
    public override async Task<GetReportStatusResponse> GetReportStatus(GetReportStatusRequest request,
        ServerCallContext context) {
        var response = new GetReportStatusResponse {
            ReportId = "0",
            Status = "ERROR",
            ErrorMessage = "not found"
        };
    
        var filter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, int.Parse(request.ReportId));
        var report = await _mongoDbContext.Reports.Find(filter).FirstOrDefaultAsync();
        if (report == null) {
            return response;
        }
        response.Status = report.Status;
        response.ReportId = report.OrderId.ToString();
        response.ErrorMessage = "";
        return response;
    }

    public override async Task<GenerateReportResponse> GenerateReport(GenerateReportRequest request,
        ServerCallContext context) {
        var response = new GenerateReportResponse {
            ReportId = request.OrderId,
            Status = "ERROR",
        };
        
        var report = await _mongoDbContext.Reports.Find(r => r.OrderId == int.Parse(request.OrderId)).FirstOrDefaultAsync();
        var reportFilePath = _pdfGeneratorService.GenerateReportPdf(string.IsNullOrEmpty(request.OrderId) ? "Collective report" : "Report", report);
        await using var fileStream = File.OpenRead(reportFilePath);
        string uploadedPath =
            await _blobStorageService.UploadImageAsync(fileStream, Path.GetFileName(reportFilePath), "application/pdf");
        
        report.ReportUrl = uploadedPath;
        report.ExpiresAt = DateTime.UtcNow.AddDays(8);
        report.Status = "GENERATED";
        
        var filter = Builders<ReportEntity>.Filter.Eq(r => r.OrderId, report.OrderId);
        var update = Builders<ReportEntity>.Update
            .Set(r => r.ReportUrl, report.ReportUrl)
            .Set(r => r.ExpiresAt, report.ExpiresAt)
            .Set(r => r.Status, report.Status);
        
        await _mongoDbContext.Reports.UpdateOneAsync(filter, update);
        
        try {
            File.Delete(reportFilePath);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, $"Could not delete temporary file: {reportFilePath}");
        }
        _logger.LogInformation($"Generated report: {report.ReportUrl}");
        response.Status = report.Status;
        _workshopStatusPublisher.PublishStatusChange(report.OrderId.ToString(), report.Status);
        return response;
    }

    public override async Task<SendEmailWithReportResponse> SendEmailWithReport(SendEmailWithReportRequest request,
        ServerCallContext context) {
        var response = new SendEmailWithReportResponse {
            Message = "Unknown error",
            Success = false,
        };

        var report = await _mongoDbContext.Reports.Find(r => r.OrderId == int.Parse(request.ReportId))
            .FirstOrDefaultAsync();
        if (report == null) {
            response.Message = "Not found";
            return response;
        }

        if (string.IsNullOrEmpty(report.ReportUrl)) {
            response.Message = "Report is not generated";
            return response;
        }

        List<UserEntity> users;
        if (request.UsersIds == null || request.UsersIds.Count == 0) {
            users = await _mongoDbContext.Users
                .Find(u => u.UserRoles.Any(r => r.Name == "admin"))
                .ToListAsync();
        }
        else {
            users = await _mongoDbContext.Users
                .Find(u => request.UsersIds.Contains(u.Id.ToString()))
                .ToListAsync();
        }
        var userEmailsAndNames = users.Select(u => new {
            Email = u.Email,
            Name = u.Name
        }).ToList();
        
        var mqMessage = new {
            ReportUrl = report.ReportUrl,
            Receivers = userEmailsAndNames
        };
        
        _reportServiceEventPublisher.PublishEvent("email.send.report_email", mqMessage);
        response.Message = "Email sent";
        response.Success = true;
        
        return response;
    }

    public override async Task<GetReportsListResponse> GetReportsList(GetReportsListRequest request,
        ServerCallContext context) {
        var response = new GetReportsListResponse {
            Reports = {}
        };
        
        var filter = Builders<ReportEntity>.Filter.In(r => r.Status, new[] { "GENERATED", "EXPIRED" });

        var reports = await _mongoDbContext.Reports
            .Find(filter)
            .Skip((request.Page - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync();

        foreach (var report in reports) {
            var dto = new Report {
                Id = report.Id.ToString(),
                Vehicle = new Vehicle {
                    Id = report.VehicleId,
                    Model = report.Vehicle.Model,
                    Year = report.Vehicle.Year,
                    Brand = report.Vehicle.Brand,
                    CreatedAt = Timestamp.FromDateTime(report.Vehicle.CreatedAt),
                },
                Mechanic = new UserDto {
                    Id = report.User.Id,
                    Name = report.User.Name,
                    Email = report.User.Email
                },
                Status = report.Status,
                CreatedAt = Timestamp.FromDateTime(report.CreatedAt),
                ExpiresAt = Timestamp.FromDateTime(report.ExpiresAt),
            };

            response.Reports.Add(dto);
        }

        return response;
    }
}