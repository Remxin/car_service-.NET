using dotenv.net;

namespace AuthService;

public class EnvConfig
{
    public string DbConnectionString { get; }

    public EnvConfig()
    {
        DotEnv.Load();

        DbConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");
        if (string.IsNullOrWhiteSpace(DbConnectionString))
            throw new Exception("Brak lub pusta zmienna środowiskowa POSTGRES_CONN_STR!");
        
    }
}
