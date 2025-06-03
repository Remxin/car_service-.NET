using dotenv.net;

namespace ReportService;

public class EnvConfig {
    public string ReportServicePort { get;  }
    public string DbConnectionString { get; }
    public string RabbitMQHost { get; }
    public string RabbitMQPort { get; }
    public string RabbitMQUserName { get; }
    public string RabbitMQPassword { get; }
    public string RabbitMQVirtualHost { get; }
    public string AzureBlobConnectionString { get; }
    public string AzureBlobContainerName { get; }
    

    public EnvConfig() {
        DotEnv.Load();
        DbConnectionString = Environment.GetEnvironmentVariable("MONGO_CONN_STR");
        if (string.IsNullOrWhiteSpace(DbConnectionString))
            throw new Exception("Unable to find environment variable MONGO_CONN_STR");
        
        ReportServicePort = Environment.GetEnvironmentVariable("REPORT_SERVICE_PORT");
        if (string.IsNullOrWhiteSpace(ReportServicePort))
            throw new Exception("Unable to find environment variable REPORT_SERVICE_PORT");
        
        RabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        if (string.IsNullOrWhiteSpace(RabbitMQHost))
            throw new Exception("Unable to find environment variable RABBITMQ_HOST");
        
        RabbitMQPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        if (string.IsNullOrWhiteSpace(RabbitMQPort))
            throw new Exception("Unable to find environment variable RABBITMQ_PORT");
        
        RabbitMQUserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        if (string.IsNullOrWhiteSpace(RabbitMQUserName))
            throw new Exception("Unable to find environment variable RABBITMQ_USERNAME");
        
        RabbitMQPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
        if (string.IsNullOrWhiteSpace(RabbitMQPassword))
            throw new Exception("Unable to find environment variable RABBITMQ_PASSWORD");
        
        RabbitMQVirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VIRTUALHOST");
        if (string.IsNullOrWhiteSpace(RabbitMQVirtualHost))
            throw new Exception("Unable to find environment variable RABBITMQ_VIRTUALHOST");
        
        AzureBlobConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONN_STR");
        if (string.IsNullOrWhiteSpace(AzureBlobConnectionString))
            throw new Exception("Unable to find environment variable AZURE_BLOB_CONN_STR");
        
        AzureBlobContainerName = Environment.GetEnvironmentVariable("AZURE_BLOB_CONTAINER_NAME");
        if (string.IsNullOrWhiteSpace(AzureBlobContainerName))
            throw new Exception("Unable to find environment variable AZURE_BLOB_CONTAINER_NAME");
    }
}