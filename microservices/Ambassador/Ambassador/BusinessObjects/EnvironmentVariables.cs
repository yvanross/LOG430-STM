namespace Ambassador.BusinessObjects;

internal static class EnvironmentVariables
{
    internal static string IngressAddress => 
        "http://" + Environment.GetEnvironmentVariable("INGRESS_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("INGRESS_PORT");
    
    internal static string ServiceAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS");
}