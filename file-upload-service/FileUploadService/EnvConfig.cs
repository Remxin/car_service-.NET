using dotenv.net;

namespace FileUploadService;

public class EnvConfig
{
    public string FileUploadServicePort { get; }
    public string AuthServiceUrl { get; }
  
    public EnvConfig() {
        DotEnv.Load();
        FileUploadServicePort = Environment.GetEnvironmentVariable("FILE_UPLOAD_SERVICE_PORT");
        if (string.IsNullOrWhiteSpace(FileUploadServicePort))
            throw new Exception("Unable to find environment variable FILE_UPLOAD_SERVICE_PORT");
        
        AuthServiceUrl = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(AuthServiceUrl))
            throw new Exception("Unable to find environment variable AUTH_SERVICE_URL");
    }
}
