using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases;

public class ChaosExperiment
{
    private readonly IPodReadService _podReadService;

    private readonly IDataStreamService _streamService;

    private readonly ILogger<ChaosExperiment> _logger;

    private readonly ResourceManagementService _resourceManagementService;
   
    private IChaosCodex _codex;

    private int _killCount;
    
    private int _hardwareFailCount;

    public ChaosExperiment(IEnvironmentClient environmentClient, IPodReadService podReadService, IPodWriteService writeServiceService, IDataStreamService streamService, IHostInfo hostInfo, ILogger<ChaosExperiment> logger)
    {
        _podReadService = podReadService;
        _streamService = streamService;
        _logger = logger;
        _resourceManagementService = new ResourceManagementService(environmentClient, podReadService, writeServiceService, hostInfo);
    }

    public async Task SendTimeComparisonRequestToPool(ICoordinates coordinates)
    {
        await _streamService.Produce(coordinates);
    }

    public void SetChaosCodex(IChaosCodex codex)
    {
        _codex = codex;
    }

    public async Task InduceChaos()
    {
        if (DateTime.UtcNow < _codex.EndTestAt && _codex.StartTestAt < DateTime.UtcNow)
        {
            var totalDuration = (GetSecondsSinceEpoch(_codex.EndTestAt) - GetSecondsSinceEpoch(_codex.StartTestAt));
            var timeSinceBeginningOfExperiment = (GetSecondsSinceEpoch(DateTime.UtcNow) - GetSecondsSinceEpoch(_codex.StartTestAt));

            var completionPercentage =
                timeSinceBeginningOfExperiment /
                totalDuration;

            foreach (var kv in _codex.ChaosConfigs.OrderBy(_ => Random.Shared.Next()))
            {
                var chaosConfig = kv.Value;
                var category = Enum.GetName(kv.Key)!;

                var sigmoid = (double time, double parameter) => (parameter / 60.0) / (1.0 + Math.Exp(-2.5 * (time / 600.0)));

                var expectedKillCountAtThisMoment = DefiniteIntegralService.TrapezoidalRule(0, timeSinceBeginningOfExperiment, sigmoid, chaosConfig.KillRate);

                var expectedHardwareFailureCountAtThisMoment = (chaosConfig.HardwareFailures / 60.0) * timeSinceBeginningOfExperiment;

                var memory = (long)(chaosConfig.Memory / completionPercentage);

                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

                await InduceHardwareFailures(category, expectedHardwareFailureCountAtThisMoment, _codex.StartTestAt);

                await ChaosMonkey(expectedKillCountAtThisMoment, chaosConfig.MaxNumberOfPods, category, _codex.StartTestAt);

                await SetResourcesOnPods(category, nanoCpus, memory);
            }
        }

        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }

    private async Task ChaosMonkey(double expectedKillCountAtThisMoment, double maxNumberOfPods, string category, DateTime codexStartTestAt)
    {
        var getPodCountOfType = _podReadService.GetAllPods().Count(pod => _podReadService.GetPodType(pod.Type)?.ServiceTypes.Any(st => st.ArtifactType.EqualsIgnoreCase(category)) ?? false);

        var overPodLimit = Math.Max(getPodCountOfType - maxNumberOfPods - expectedKillCountAtThisMoment, 0);

        expectedKillCountAtThisMoment = Math.Floor(expectedKillCountAtThisMoment) + overPodLimit;

        for (var i = _killCount; i < expectedKillCountAtThisMoment; i++)
        {
            var podsOfType = _podReadService.GetAllPodTypes()
                .FindAll(podType => podType.ServiceTypes
                    .Any(serviceType => serviceType.ArtifactType.Equals(category)))
                .ToDictionary(p => p.Type);

            var podToKill = _podReadService.GetAllPods()
                .Where(pod => podsOfType.ContainsKey(pod.Type))
                .Where(pod => pod.ServiceStatus is ReadyState)
                .MinBy(pod => codexStartTestAt > pod.ServiceStatus.GetStateChangeTime() ? codexStartTestAt  :  pod.ServiceStatus.GetStateChangeTime());

            if (podToKill is not null)
            {
                var softKill = category.EqualsIgnoreCase(Enum.GetName(ArtifactTypeEnum.Connector)) || category.EqualsIgnoreCase(Enum.GetName(ArtifactTypeEnum.Database));

                await _resourceManagementService.RemovePodInstance(podToKill, softKill);

                _killCount++;
            }
        }
    }

    private async Task InduceHardwareFailures(string category, double expectedHardwareFailureCountAtThisMoment, DateTime codexStartTestAt)
    {
        for (int i = _hardwareFailCount; i < Math.Floor(expectedHardwareFailureCountAtThisMoment); i++)
        {
            var servicesOfCurrentType = _podReadService.GetAllServiceTypes()
                .Where(st => st.ArtifactType.EqualsIgnoreCase(category))
                .ToDictionary(st=>st.Type);

            var serviceInstances = _podReadService.GetAllServices()
                .Where(serviceInstance => servicesOfCurrentType.ContainsKey(serviceInstance.Type))
                .OrderBy(serviceInstance => codexStartTestAt > serviceInstance.ServiceStatus.GetStateChangeTime() ? codexStartTestAt : serviceInstance.ServiceStatus.GetStateChangeTime())
                .SelectMany(serviceInstance => serviceInstance.VolumeIds)
                .Where(volumeId => string.IsNullOrWhiteSpace(volumeId) is false)
                .ToList();

            var volumeToFailName = serviceInstances.FirstOrDefault();

            if (volumeToFailName is not null)
            {
                var servicesToKill = 
                    _podReadService.GetAllServices()
                    .Where(si => si.VolumeIds.Contains(volumeToFailName))
                    .ToList();

                foreach (var serviceInstance in servicesToKill)
                {
                    var pod = _podReadService.GetPodOfService(serviceInstance);

                    if (pod is not null)
                    {
                        var serviceType = _podReadService.GetServiceType(serviceInstance.Type);

                        serviceType?.ContainerConfig.Config.Mounts?.Clear();

                        var softKill = category.EqualsIgnoreCase(Enum.GetName(ArtifactTypeEnum.Connector)) || category.EqualsIgnoreCase(Enum.GetName(ArtifactTypeEnum.Database));

                        await _resourceManagementService.RemovePodInstance(pod, softKill);
                    }
                }

                await _resourceManagementService.RemoveVolume(volumeToFailName);

                if (servicesToKill.Any())
                {
                    _hardwareFailCount++;
                }
            }
        }
    }

    private async Task SetResourcesOnPods(string? category, long nanoCpus, long memory)
    {
        var podsOfTypeDict = _podReadService.GetAllPodTypes()
            .FindAll(podType => podType.ServiceTypes
                .Any(serviceType => serviceType.ArtifactType.Equals(category)))
            .ToDictionary(p => p.Type);

        var pod = _podReadService.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type))
            .MinBy(_ => Random.Shared.Next());

        if (pod is not null)
            await _resourceManagementService.SetResources(pod, nanoCpus > 100000000 ? 0 : nanoCpus, memory > 2000000000 ? 0 : memory);
    }
}