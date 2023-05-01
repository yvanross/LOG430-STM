using ApplicationLogic.Interfaces;

namespace Configuration;

public class HostInfo : IHostInfo
{
    private const string Version = "1.0.0";

    private const bool CheatsAllowed = true;

    private static readonly string Group = Environment.GetEnvironmentVariable("GROUP")!;

    private static readonly string TeamName = Environment.GetEnvironmentVariable("TEAM_NAME")!;

    private static readonly string IngressAddress = Environment.GetEnvironmentVariable("INGRESS_ADDRESS")!;

    private static readonly string IngressPort = Environment.GetEnvironmentVariable("INGRESS_PORT")!;

    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    private static readonly string MqServiceName = Environment.GetEnvironmentVariable("MQ_SERVICE_NAME")!;

    private static readonly string NodeControllerPort = Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT")!;

    private static readonly string CustomContainerPortsDiscovery = Environment.GetEnvironmentVariable("CUSTOM_CONTAINER_PORTS_DISCOVERY")!;

    private static readonly string Secret = Environment.GetEnvironmentVariable("SECRET")!;

    private static readonly string Username = Environment.GetEnvironmentVariable("USERNAME")!;

    private static readonly string BridgePort = Environment.GetEnvironmentVariable("BRIDGE_PORT")!;

    public string GetTeamName()
    {
        return TeamName;
    }

    public string GetIngressAddress()
    {
        return IngressAddress;
    }

    public string GetIngressPort()
    {
        return IngressPort;
    }

    public string GetAddress()
    {
        return ServiceAddress;
    }

    public string GetPort()
    {
        return NodeControllerPort;
    }

    public string GetMQServiceName()
    {
        return MqServiceName;
    }

    public string GetCustomContainerPorts()
    {
        return CustomContainerPortsDiscovery;
    }

    public bool GetCheatsAllowed()
    {
        return CheatsAllowed;
    }

    public string GetUsername()
    {
        return Username;
    }

    public string GetSecret()
    {
        return Secret;
    }

    public string GetVersion()
    {
        return Version;
    }

    public string GetBridgePort()
    {
        return BridgePort;
    }

    public string GetGroup()
    {
        return Group;
    }
}