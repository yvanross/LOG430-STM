using ApplicationLogic.Interfaces;
using Entities.Dao;

namespace ApplicationLogic.Usecases;

public class PlannedResourcesUpdate
{
    private readonly IPodReadService _podReadService;
    private readonly IEnvironmentClient _environmentClient;
    private readonly IHostInfo _hostInfo;

    public PlannedResourcesUpdate(IPodReadService podReadService, IEnvironmentClient environmentClient, IHostInfo hostInfo)
    {
        _podReadService = podReadService;
        _environmentClient = environmentClient;
        _hostInfo = hostInfo;
    }

    //todo not thread safe
    public void IncreaseNumberOfPod(string? podTypeName)
    {
        if (_podReadService.GetPodType(podTypeName) is { } podType)
            podType.IncreaseNumberOfPod();
    }

    //todo not thread safe
    public void DecreaseNumberOfPod(string? podTypeName)
    {
        if (_podReadService.GetPodType(podTypeName) is { } podType)
            podType.DecreaseNumberOfPod();
    }

    public void SetNumberOfPod(string? podTypeName, int numberOfInstances)
    {
        if (_podReadService.GetPodType(podTypeName) is { } podType)
            podType.SetNumberOfPod(numberOfInstances);
    }

    //expensive action
    public void SetGlobalCpu(long nanoCpus)
    {
        if (_hostInfo.GetIsDirty() is false)
        {
            _podReadService.GetAllPods().ForEach(pod => _environmentClient.SetResources(pod, nanoCpus));
        }
    }
}