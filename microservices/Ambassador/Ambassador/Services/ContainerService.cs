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

    internal static string NodeControllerAddress =>
        "http://" + Environment.GetEnvironmentVariable("INGRESS_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("INGRESS_PORT");

    internal static string ServiceAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    internal static readonly int RetryCount = 10;
}