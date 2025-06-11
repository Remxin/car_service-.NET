using dotenv.net;

namespace AuthService;

public class EnvConfig
{
    public string DbConnectionString { get; }
    public string JwtSecretKey { get; }
    public string AuthServicePort { get; }
    public string RabbitMQHost { get; }
    public string RabbitMQPort { get; }
    public string RabbitMQUserName { get; }
    public string RabbitMQPassword { get; }
    public string RabbitMQVirtualHost { get; }

    public EnvConfig()
    {
        DotEnv.Load();

        DbConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");
        if (string.IsNullOrWhiteSpace(DbConnectionString))
            throw new Exception("Brak lub pusta zmienna środowiskowa POSTGRES_CONN_STR!");

        JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrWhiteSpace(JwtSecretKey))
            throw new Exception("Brak lub pusta zmienna środowiskowa JWT_SECRET_KEY!");

        AuthServicePort = Environment.GetEnvironmentVariable("AUTH_SERVICE_PORT");
        if (string.IsNullOrWhiteSpace(JwtSecretKey))
            throw new Exception("Brak lub pusta zmienna środowiskowa AUTH_SERVICE_PORT!");

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
    }
}
