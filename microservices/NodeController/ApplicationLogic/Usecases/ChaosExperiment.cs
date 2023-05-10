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

                var func = (double x) => (chaosConfig.KillRate / 60.0) * (x / totalDuration);

                var expectedKillCountAtThisMoment = DefiniteIntegralService.TrapezoidalRule(0, currentPoint, func);

                var memory = (long)(chaosConfig.Memory / completionPercentage);

                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

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

    private async Task SetResourcesOnPods(string? category, long nanoCpus, long memory)
    {
        var podsOfTypeDict = _readServiceService.GetAllPodTypes()
            .FindAll(podType => podType.ServiceTypes
                .Any(serviceType => serviceType.ArtifactType.Equals(category)))
            .ToDictionary(p => p.Type);

        var pod = _readServiceService.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type))
            .MinBy(_ => Random.Shared.Next());

        if (pod is not null)
            await _resourceManagementService.SetResources(pod, nanoCpus > 100000000 ? 0 : nanoCpus, memory > 200000000 ? 0 : memory);
    }
}