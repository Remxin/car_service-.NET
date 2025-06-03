using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ReportService.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(string connectionString, string containerName, ILogger<BlobStorageService> logger)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.None);
        _logger = logger;
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(imageStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        });

        _logger.LogInformation($"Uploaded image: {fileName}");
        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
    
    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var downloadResult = await blobClient.DownloadAsync();
        return downloadResult.Value.Content;
    }
    
    public Task<(string sasUrl, DateTime expiresAt)> GenerateDownloadLinkAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
        
        if (!blobClient.CanGenerateSasUri) {
            throw new InvalidOperationException("BlobClient cannot generate SAS URI. Check if you're using a credentialed client.");
        }

        var sasBuilder = new BlobSasBuilder {
            BlobContainerName = _containerClient.Name,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = expiresAt
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        _logger.LogInformation($"Generated SAS URI: {sasUri}");
        return Task.FromResult((sasUri.ToString(), expiresAt.UtcDateTime));
    }
}