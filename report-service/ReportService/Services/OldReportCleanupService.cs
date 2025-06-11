using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ReportService.Data;
using ReportService.Entities;
using ReportService.Services;
using Shared.Grpc.Services;

namespace ReportService.Services;
public class OldReportCleanupService(
    BlobStorageService blobStorageService,
    ILogger<OldReportCleanupService> logger,
    WorkshopStatusPublisher workshopStatusPublisher,
    MongoDbContext mongoDbContext
    ) : BackgroundService {
    private readonly BlobStorageService _blobStorageService = blobStorageService;
    private readonly ILogger<OldReportCleanupService> _logger = logger;
    private readonly IWorkshopStatusPublisher _workshopStatusPublisher = workshopStatusPublisher;
    private readonly MongoDbContext _mongoDbContext = mongoDbContext;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = DateTime.UtcNow.Date.AddDays(1).AddHours(1); // jutro o 1:00 UTC
            var delay = nextRun - now;

            _logger.LogInformation($"Cleanup scheduled in {delay.TotalMinutes} minutes");

            await Task.Delay(delay, stoppingToken);

            try {
                await CleanupOldFilesAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error while cleaning up old blobs.");
            }
        }
    }
    private static string GetBlobNameFromUrl(string url) {
        return new Uri(url).Segments.Last();
    }

    private async Task CleanupOldFilesAsync() {
        _logger.LogInformation("⏳ Starting cleanup of expired reports...");

        var filter = Builders<ReportEntity>.Filter.Lt(r => r.ExpiresAt, DateTime.UtcNow);
        var expiredReports = await _mongoDbContext.Reports.Find(filter).ToListAsync();

        foreach (var report in expiredReports) {
            if (string.IsNullOrWhiteSpace(report.ReportUrl))
                continue;

            var blobName = GetBlobNameFromUrl(report.ReportUrl);
            _logger.LogInformation($"🗑 Deleting expired report blob: {blobName}");

            await _blobStorageService.DeleteImageAsync(blobName);
            
            var update = Builders<ReportEntity>.Update
                .Set(r => r.ReportUrl, null)
                .Set(r => r.Status, "EXPIRED");

            await _mongoDbContext.Reports.UpdateOneAsync(
                Builders<ReportEntity>.Filter.Eq(r => r.Id, report.Id),
                update
            );
            _workshopStatusPublisher.PublishStatusChange(report.OrderId.ToString(), "EXPIRED");
        }

        _logger.LogInformation("✅ Expired report cleanup finished.");
    }
}