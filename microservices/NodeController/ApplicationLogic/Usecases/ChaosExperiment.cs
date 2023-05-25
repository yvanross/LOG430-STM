using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases;

public class ChaosExperiment
{
    private readonly IPodReadService _readServiceService;

    private readonly IDataStreamService _streamService;

    private readonly ILogger<ChaosExperiment> _logger;

    private readonly ResourceManagementService _resourceManagementService;
   
    private IChaosCodex _codex;

    private int _killCount;
    
    private int _hardwareFailCount;

    public ChaosExperiment(IEnvironmentClient environmentClient, IPodReadService readServiceService, IPodWriteService writeServiceService, IDataStreamService streamService, ILogger<ChaosExperiment> logger)
    {
        _readServiceService = readServiceService;
        _streamService = streamService;
        _logger = logger;
        _resourceManagementService = new ResourceManagementService(environmentClient, readServiceService, writeServiceService);
    }

    public async Task SendTimeComparisonRequestToPool(ICoordinates coordinates)
    {
        await _streamService.Produce(coordinates);
    }

    public void SetChaosCodex(IChaosCodex codex) => _codex = codex;

    public async Task InduceChaos()
    {
        if (DateTime.UtcNow < _codex.EndTestAt && _codex.StartTestAt < DateTime.UtcNow)
        {
            var totalDuration = (GetSecondsSinceEpoch(_codex.EndTestAt) - GetSecondsSinceEpoch(_codex.StartTestAt));
            var currentPoint = (GetSecondsSinceEpoch(DateTime.UtcNow) - GetSecondsSinceEpoch(_codex.StartTestAt));

            var completionPercentage =
                currentPoint /
                totalDuration;

            foreach (var kv in _codex.ChaosConfigs.OrderBy(_ => Random.Shared.Next()))
            {
                var chaosConfig = kv.Value;
                var category = Enum.GetName(kv.Key);

                var func = (double time, double parameter) => (parameter / 60.0) * (time / totalDuration);

                var expectedKillCountAtThisMoment = DefiniteIntegralService.TrapezoidalRule(0, currentPoint, func, chaosConfig.KillRate);
                
                var expectedHardwareFailureCountAtThisMoment = DefiniteIntegralService.TrapezoidalRule(0, currentPoint, func, chaosConfig.HardwareFailures);

                var memory = (long)(chaosConfig.Memory / completionPercentage);

                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

                await InduceHardwareFailures(category, expectedHardwareFailureCountAtThisMoment);

                await ChaosMonkey(expectedKillCountAtThisMoment, chaosConfig.MaxNumberOfPods, category);

                await SetResourcesOnPods(category, nanoCpus, memory);
            }
        }

        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }

    private async Task ChaosMonkey(double expectedKillCountAtThisMoment, double maxNumberOfPods, string? category)
    {
        var overPodLimit = Math.Max(_readServiceService.GetAllPods().Count - maxNumberOfPods - expectedKillCountAtThisMoment, 0);

        expectedKillCountAtThisMoment += overPodLimit;

        int maxKills = Convert.ToInt32(expectedKillCountAtThisMoment);

        for (var i = _killCount; i < maxKills; i++)
        {
            var podsOfType = _readServiceService.GetAllPodTypes()
                .FindAll(podType => podType.ServiceTypes
                    .Any(serviceType => serviceType.ArtifactType.Equals(category)))
                .ToDictionary(p => p.Type);

            var podToKill = _readServiceService.GetAllPods()
                .Where(pod => podsOfType.ContainsKey(pod.Type))
                .MinBy(_ => Random.Shared.Next());

            if (podToKill is not null)
            {
                await _resourceManagementService.RemovePodInstance(podToKill);

                _killCount++;
            }
        }
    }

    private async Task InduceHardwareFailures(string? category, double expectedHardwareFailureCountAtThisMoment)
    {
        for (int i = _hardwareFailCount; i < expectedHardwareFailureCountAtThisMoment; i++)
        {
            var serviceTypeDict = _readServiceService.GetAllServiceTypes()
                .Where(serviceType => serviceType.ArtifactType.Equals(category))
                .ToDictionary(p => p.Type);

            var volumeToFailName = serviceTypeDict
                .Select(s => s.Value.ContainerConfig.Config.Mounts
                    .MinBy(_ => Random.Shared.Next())?.Name)
                .OfType<string>()
                .MinBy(_ => Random.Shared.Next());

            if (volumeToFailName is not null)
            {
                var serviceTypesToKill = _readServiceService.GetAllServiceTypes()
                    .Where(st => st.ContainerConfig.Config.Mounts
                        .Any(mount => mount.Name.Equals(volumeToFailName, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                var services = _readServiceService.GetAllServices()
                    .Where(s => serviceTypesToKill
                        .Any(st => st.Type.Equals(s.Type, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                foreach (var serviceToKill in services)
                {
                    var pod = _readServiceService.GetPodOfService(serviceToKill);

                    if (pod is not null)
                    {
                        await _resourceManagementService.RemovePodInstance(pod);
                    }
                }

                await _resourceManagementService.RemoveVolume(volumeToFailName);

                if (services.Any())
                {
                    _hardwareFailCount++;
                }
            }
        }
    }

    private async Task SetResourcesOnPods(string? category, long nanoCpus, long memory)
    {
        var podsOfTypeDict = _readServiceService.GetAllPodTypes()
            .FindAll(podType => podType.ServiceTypes
                .Any(serviceType => serviceType.ArtifactType.Equals(category)))
            .ToDictionary(p => p.Type);

        var pod = _readServiceService.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type))
            .MinBy(_ => Random.Shared.Next());

        if (pod is not null)
            await _resourceManagementService.SetResources(pod, nanoCpus > 100000000 ? 0 : nanoCpus, memory > 2000000000 ? 0 : memory);
    }
}