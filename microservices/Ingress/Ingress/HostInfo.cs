using ApplicationLogic.Interfaces;

namespace NodeController.External.Docker;

public class HostInfo : IHostInfo
{
    public static readonly string NodeStateStoragePort = Environment.GetEnvironmentVariable("NODE_STATE_STORAGE_PORT")!;

    public static readonly string ExperimentHistoryStoragePort = Environment.GetEnvironmentVariable("EXPERIMENT_HISTORY_STORAGE_PORT")!;

    public static readonly string ServiceAddress = Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

    public string GetNodeStateStoragePort()
    {
        return NodeStateStoragePort;
    }

    public string GetExperimentHistoryStoragePort()
    {
        return ExperimentHistoryStoragePort;
    }

    public string GetAddress()
    {
        return ServiceAddress;
    }
}