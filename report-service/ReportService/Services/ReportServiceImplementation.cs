using Google.Protobuf.WellKnownTypes;
using ReportService.Data;
using Grpc.Core;
using MongoDB.Driver;
using ReportService;
using ReportService.Entities;
using Shared.Grpc.Messages;

namespace ReportService.Services;


public class ReportServiceImplementation(
    BlobStorageService blobStorageService,
    MongoDbContext mongoDbContext,
    PdfGeneratorService pdfGeneratorService,
    ReportServiceEventPublisher reportServiceEventPublisher
    ) : Shared.Grpc.Services.ReportService.ReportServiceBase {
    private readonly BlobStorageService _blobStorageService = blobStorageService;
    private readonly MongoDbContext _mongoDbContext = mongoDbContext;
    private readonly PdfGeneratorService _pdfGeneratorService = pdfGeneratorService;
    private readonly ReportServiceEventPublisher _reportServiceEventPublisher = reportServiceEventPublisher;
    
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
        
        response.Status = report.Status;
        return response;
    }

    public override async Task<SendEmailWithReportResponse> SendEmailWithReport(SendEmailWithReportRequest request,
        ServerCallContext context) {
        var response = new SendEmailWithReportResponse {
            Message = "Unknown error",
            Success = false,
        };
        
        var report = await _mongoDbContext.Reports.Find(r => r.OrderId == int.Parse(request.ReportId)).FirstOrDefaultAsync();
        if (report == null) {
            response.Message = "Not found";
            return response;
        }

        if (string.IsNullOrEmpty(report.ReportUrl)) {
            response.Message = "Report is not generated";
        }

        var users = await _mongoDbContext.Users
            .Find(u => request.UsersIds.Contains(u.Id.ToString()))
            .ToListAsync();
        
        var mqMessage = new {
            ReportUrl = report.ReportUrl,
            Receivers = users
        };
        
        _reportServiceEventPublisher.PublishEvent("email.send.report_email", mqMessage);
        response.Message = "Email sent";
        response.Success = true;
        
        return response;
    }
}