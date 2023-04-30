using ApplicationLogic.Interfaces;

namespace Configuration;

public class HostInfo : IHostInfo
{
    private static readonly string NodeStateStoragePort = Environment.GetEnvironmentVariable("NODE_STATE_STORAGE_PORT")!;

    private static readonly string AuthServicePort = Environment.GetEnvironmentVariable("AUTH_SERVICE_PORT")!;

    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    private static readonly string BridgePort = Environment.GetEnvironmentVariable("BRIDGE_PORT")!;

    public string GetNodeStateStoragePort()
    {
        return NodeStateStoragePort;
    }

    public string GetAuthServicePort()
    {
        return AuthServicePort;
    }

    public string GetAddress()
    {
        return ServiceAddress;
    }

    public string GetBridgePort()
    {
        return BridgePort;
    }
}