using Microsoft.Extensions.Options;

namespace ServiceMeshHelper.Services;

public static class ServiceMeshConfiguration
{
    private static string? _nodeControllerPort;
    private static string? _servicesAddress;

    internal static string ServiceId => Environment.GetEnvironmentVariable("ID") ?? throw new Exception("ID not set as an env variable on container.");

    internal static string NodeControllerPort
    {
        get => _nodeControllerPort ?? Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT") ?? "32770";
        set => _nodeControllerPort = value;
    }

    internal static string ServicesAddress
    {
        get => _servicesAddress ?? Environment.GetEnvironmentVariable("SERVICES_ADDRESS") ?? "host.docker.internal";
        set => _servicesAddress = value;
    }

    internal static string NodeControllerAddress => $"http://{ServicesAddress}:{NodeControllerPort}";

    public static void Configure(ServiceMeshOptions options)
    {
        _nodeControllerPort = options.NodeControllerPort;
        _servicesAddress = options.ServicesAddress;
    }
}