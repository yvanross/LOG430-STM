namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetTeamName();

    string GetIngressAddress();

    string GetIngressPort();

    string GetAddress();

    string GetPort();
    
    string GetMQServiceName();
    
    string GetCustomContainerPorts();
    
    bool GetCheatsAllowed();

    string GetUsername();

    string GetSecret();

    string GetVersion();

    string GetBridgePort();
}