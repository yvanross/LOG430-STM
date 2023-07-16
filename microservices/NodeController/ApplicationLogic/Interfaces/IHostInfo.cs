namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    bool IsIngressConfigValid();

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
    
    string GetGroup();
    
    bool GetSecure();
    
    public bool GetIsDirty();

    public void SetIsDirty(bool dirty);

    string GetContainerId();

    IEnumerable<int> GetTunnelPortRange();
}