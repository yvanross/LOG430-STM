using Application.Interfaces;

namespace Configuration;

public class HostInfo : IHostInfo
{
    private static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;
    
    private static readonly string MqServiceName = Environment.GetEnvironmentVariable("MQ_SERVICE_NAME")!;

    private static readonly string NodeControllerPort = Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT")!;

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
}