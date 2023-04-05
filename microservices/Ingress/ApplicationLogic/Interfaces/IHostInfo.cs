namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetNodeStateStoragePort();

    string GetExperimentHistoryStoragePort();

    string GetAddress();
}