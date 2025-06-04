using dotenv.net;

namespace EmailService;

public class EnvConfig {
    public string EmailServicePort { get; }
    public string RabbitMQHost { get; }
    public string RabbitMQPort { get; }
    public string RabbitMQUserName { get; }
    public string RabbitMQPassword { get; }
    public string RabbitMQVirtualHost { get; }
    public string AzureBlobConnectionString { get; }
    public string AzureBlobContainerName { get; }
    public string EmailUser { get; }
    public string EmailPassword { get; }
    
    

    public EnvConfig() {
        DotEnv.Load();
        EmailServicePort = Environment.GetEnvironmentVariable("EMAIL_SERVICE_PORT");
        if (string.IsNullOrWhiteSpace(EmailServicePort))
            throw new Exception("Unable to find environment variable EMAIL_SERVICE_PORT");
        
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
        
        EmailUser = Environment.GetEnvironmentVariable("EMAIL_USER");
        if (string.IsNullOrWhiteSpace(EmailUser))
            throw new Exception("Unable to find environment variable EMAIL_USER");
        
        EmailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        if (string.IsNullOrWhiteSpace(EmailPassword))
            throw new Exception("Unable to find environment variable EMAIL_PASSWORD");
    }
}