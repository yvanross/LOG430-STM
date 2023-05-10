namespace ServiceMeshHelper.Services;

internal class ContainerService
{
    private static string? _serviceId;

    internal static string ServiceId
    {
        get
        {
            if (_serviceId is not null) return _serviceId;
            
            if (Environment.GetEnvironmentVariable("ID") is { } stringId)
            {
                _serviceId = stringId;

                return _serviceId;
            }

            _serviceId = Guid.NewGuid().ToString();

            return _serviceId;
        }
    }

    internal static string NodeControllerAddress =>
        "http://" + Environment.GetEnvironmentVariable("SERVICES_ADDRESS") + ":" +
        Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT");
}