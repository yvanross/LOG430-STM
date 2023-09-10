using ApplicationLogic.Interfaces;
using ApplicationLogic.Usecases;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Configuration;

public class HostInfo : IHostInfo
{
    private const string Version = "3.1.1";

    private const bool CheatsAllowed = true;

    private static readonly string Group = Environment.GetEnvironmentVariable("GROUP") ?? throw new Exception("GROUP env variable is not set");

    private static readonly string TeamName = Environment.GetEnvironmentVariable("TEAM_NAME") ?? throw new Exception("TEAM_NAME env variable is not set");

    private static readonly string IngressAddress = Environment.GetEnvironmentVariable("INGRESS_ADDRESS") ?? throw new Exception("INGRESS_ADDRESS env variable is not set");

    private static readonly string IngressPort = Environment.GetEnvironmentVariable("INGRESS_PORT") ?? throw new Exception("INGRESS_PORT env variable is not set");

    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS") ?? throw new Exception("SERVICES_ADDRESS env variable is not set");

    private static readonly string MqServiceName = Environment.GetEnvironmentVariable("MQ_SERVICE_NAME") ?? throw new Exception("MQ_SERVICE_NAME env variable is not set");

    private static readonly string NodeControllerPort = Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT") ?? throw new Exception("NODE_CONTROLLER_PORT env variable is not set");

    private static readonly string CustomContainerPortsDiscovery = Environment.GetEnvironmentVariable("CUSTOM_CONTAINER_PORTS_DISCOVERY") ?? throw new Exception("CUSTOM_CONTAINER_PORTS_DISCOVERY env variable is not set");

    private static readonly string Secret = Environment.GetEnvironmentVariable("SECRET") ?? throw new Exception("SECRET env variable is not set");

    private static readonly string Username = Environment.GetEnvironmentVariable("STUDENT_NAME") ?? throw new Exception("STUDENT_NAME env variable is not set");

    private static readonly string BridgePort = Environment.GetEnvironmentVariable("BRIDGE_PORT") ?? throw new Exception("BRIDGE_PORT env variable is not set");

    private static readonly string ContainerId;

    private static readonly IEnumerable<int> TunnelPortRange = Enumerable.Range(4300, 10);

    private static bool _dirty = false;
    
    static HostInfo()
    {
        var cgroupFilePath = "/proc/self/cgroup";

        if (File.Exists(cgroupFilePath) is false) throw new Exception("cgroup file does not exist, couldn't set Container Id");

        var lines = File.ReadAllLines(cgroupFilePath);

        var containerIdLine = lines.FirstOrDefault(line => line.Contains("docker"));

        if (containerIdLine == null) throw new Exception("cgroup file does not contain docker container id, couldn't set Container Id");

        var match = Regex.Match(containerIdLine, @"[0-9a-fA-F]{64}");

        if (!match.Success) throw new Exception("container Id Line does not contain docker container id, couldn't set Container Id");

        ContainerId = match.Value;
    }

    public bool IsIngressConfigValid()
    {
        return (string.IsNullOrWhiteSpace(GetIngressAddress()) ||
                string.IsNullOrWhiteSpace(GetIngressPort()) ||
                string.IsNullOrWhiteSpace(GetBridgePort())) is false;
    }

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

    public bool GetSecure()
    {
        return ServicePoolDiscovery.BannedIds.Any() is false;
    }

    public bool GetIsDirty()
    {
        return _dirty;
    }

    public void SetIsDirty(bool dirty)
    {
        _dirty = dirty;
    }

    public string GetContainerId()
    {
        return ContainerId;
    }

    public IEnumerable<int> GetTunnelPortRange()
    {
        return TunnelPortRange;
    }
}