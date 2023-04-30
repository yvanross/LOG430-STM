namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetNodeStateStoragePort();

    string GetAuthServicePort();

    string GetAddress();

    string GetBridgePort();
}