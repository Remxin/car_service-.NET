using dotenv.net;

namespace AuthService;

public class EnvConfig
{
    public string DbConnectionString { get; }
    public string JwtSecretKey { get; }
    public string AuthServicePort { get; }

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

    }
}
