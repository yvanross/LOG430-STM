using Ambassador.BusinessObjects;
using Microsoft.Extensions.Logging;

namespace Ambassador.Health;

internal class ContainerService
{
    internal static readonly Guid ServiceId = Guid.NewGuid();

    internal static ILogger? Logger;

    internal static string IngressAddress =>
        "http://" + Environment.GetEnvironmentVariable("INGRESS_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("INGRESS_PORT");

    internal static string ServiceAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    internal static readonly int RetryCount = 5;

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
            Logger?.LogError("Unable to determine the container ID. Service not connected to Ingress");

            throw new NullReferenceException("container id was null");
        }

        return containerId;
    }
}