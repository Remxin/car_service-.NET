using dotenv.net;

namespace Gateway.Api;

public class EnvConfig {
    public string GatewayPort { get; }
    public string AuthServiceUrl { get; }
    public string WorkshopServiceUrl { get; }
    public string ReportServiceUrl { get;  }
    
    public EnvConfig() {
        DotEnv.Load();
        
        GatewayPort = Environment.GetEnvironmentVariable("GATEWAY_PORT");
        if (string.IsNullOrWhiteSpace(GatewayPort))
            throw new Exception("Unable to find environment variable GATEWAY_PORT");
        AuthServiceUrl = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(AuthServiceUrl))
            throw new Exception("Unable to find environment variable AUTH_SERVICE_URL");
        
        WorkshopServiceUrl = Environment.GetEnvironmentVariable("WORKSHOP_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(WorkshopServiceUrl))
            throw new Exception("Unable to find environment variable WORKSHOP_SERVICE_URL");
        
        ReportServiceUrl = Environment.GetEnvironmentVariable("REPORT_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(ReportServiceUrl))
            throw new Exception("Unable to find environment variable REPORT_SERVICE_URL");
    }
}