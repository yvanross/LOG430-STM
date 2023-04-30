namespace AuthService;

public class HostInfo
{
    public static readonly string PostgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT")!;

    public static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;
}