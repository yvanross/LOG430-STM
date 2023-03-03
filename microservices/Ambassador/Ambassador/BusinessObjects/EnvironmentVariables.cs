using Microsoft.Extensions.Logging;

namespace Ambassador.BusinessObjects;

public static class EnvironmentVariables
{
    internal static ILogger? Logger;

    internal static string IngressAddress => 
        "http://" + Environment.GetEnvironmentVariable("INGRESS_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("INGRESS_PORT");

    public static string ServiceAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;
}