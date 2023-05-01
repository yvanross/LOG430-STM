namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetNodeStateStorageToken();

    string GetNodeStateStoragePort();

    string GetAuthServicePort();

    string GetAddress();

    string GetBridgePort();
}