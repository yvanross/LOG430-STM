namespace TripComparator.External;

public class HostInfo
{
    public static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;
    
    public static readonly string MqServiceName = Environment.GetEnvironmentVariable("MQ_SERVICE_NAME")!;

    public static readonly string NodeControllerPort = Environment.GetEnvironmentVariable("NODE_CONTROLLER_PORT")!;

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