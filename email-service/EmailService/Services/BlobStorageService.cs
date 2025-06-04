using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace EmailService.Services;
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
    
    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var downloadResult = await blobClient.DownloadAsync();
        return downloadResult.Value.Content;
    }
    
    public async Task<Stream> GetFileStreamAsync(string blobName) {
        var blobClient = _containerClient.GetBlobClient(Path.GetFileName(blobName));

        if (!await blobClient.ExistsAsync()) {
            _logger.LogWarning("❌ Blob not found: {BlobName}", blobName);
            throw new FileNotFoundException($"Blob '{blobName}' not found.");
        }

        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        memoryStream.Position = 0;

        _logger.LogInformation("📥 Blob downloaded: {BlobName}, Size: {Size} bytes", blobName, memoryStream.Length);
        return memoryStream;
    }
    
}