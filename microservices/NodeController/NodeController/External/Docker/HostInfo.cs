using ApplicationLogic.Interfaces;

namespace NodeController.External.Docker;

public class HostInfo : IHostInfo
{
    public const bool CheatsAllowed = true;

    public static readonly string TeamName = Environment.GetEnvironmentVariable("TEAM_NAME")!;

    public static readonly string IngressAddress = Environment.GetEnvironmentVariable("INGRESS_ADDRESS")!;

    public static readonly string IngressPort = Environment.GetEnvironmentVariable("INGRESS_PORT")!;

    public static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;
    
    public static readonly string MqServiceName = Environment.GetEnvironmentVariable("MQ_SERVICE_NAME")!;

    public static readonly string NodeControllerPort = Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT")!;
    
    public static readonly string CustomContainerPortsDiscovery = Environment.GetEnvironmentVariable("CUSTOM_CONTAINER_PORTS_DISCOVERY")!;

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
}