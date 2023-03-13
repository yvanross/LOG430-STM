using Ambassador.BusinessObjects;
using Microsoft.Extensions.Logging;

namespace Ambassador.Health;

internal class ContainerService
{
    private static Guid? _serviceId = null;

    internal static Guid ServiceId
    {
        get
        {
            if (_serviceId is not null) return _serviceId.Value;
            
            if (Environment.GetEnvironmentVariable("ID") is { } stringId)
            {
                Guid.TryParse(stringId, out var id);
                
                _serviceId = id;

                return _serviceId.Value;
            }

            _serviceId = Guid.NewGuid();

            return _serviceId.Value;
        }
    }

    internal static ILogger? Logger;

    internal static string IngressAddress =>
        "http://" + Environment.GetEnvironmentVariable("INGRESS_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("INGRESS_PORT");

    internal static string ServiceAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    internal static readonly int RetryCount = 10;

    internal static string SubscriptionController = "Subscription";
    
    internal static string IngressController = "Ingress";

    internal static async Task<string> GetContainerId()
    {
        // Read the contents of the metadata endpoint file
        var metadataEndpoint = "/proc/self/cgroup";

        var metadata = await File.ReadAllTextAsync(metadataEndpoint);

        // Extract the container ID from the metadata
        var containerId = metadata.Split('\n')
            .FirstOrDefault(line => line.Contains("docker"))
            ?.Split('/')
            .LastOrDefault();

        if (containerId is null)
        {
            Logger?.LogError("Unable to determine the container ID. Service not connected to IngressController");

            throw new NullReferenceException("container id was null");
        }

        return containerId;
    }

    internal static string FormatIngressRequest(string controller, string endpoint)
    {
        return $"{controller}/{ServiceAddress}/{endpoint}";
    }
}