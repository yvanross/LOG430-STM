using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class ChaosExperimentUC
{
    public int KillsThisMinute { get; private set; }

    private readonly IPodReadService _readServiceService;

    private readonly IChaosCodex _codex;
    
    private readonly IDataStreamService _streamService;

    private readonly ResourceManagementService _resourceManagementService;
    
    public ChaosExperimentUC(IEnvironmentClient environmentClient, IPodReadService readServiceService, IPodWriteService writeServiceService,
        IChaosCodex codex, IDataStreamService streamService)
    {
        _readServiceService = readServiceService;
        _codex = codex;
        _streamService = streamService;
        _resourceManagementService = new ResourceManagementService(environmentClient, readServiceService, writeServiceService);
    }

    public async Task SendTimeComparisonRequestToPool(ICoordinates coordinates)
    {
        await _streamService.Produce(coordinates);
    }

    public async Task InduceChaos()
    {
        if (DateTime.UtcNow < _codex.EndTestAt && _codex.StartTestAt < DateTime.UtcNow)
        {
            var completionPercentage =
                (GetSecondsSinceEpoch(DateTime.UtcNow) - GetSecondsSinceEpoch(_codex.StartTestAt)) /
                (GetSecondsSinceEpoch(_codex.EndTestAt) - GetSecondsSinceEpoch(_codex.StartTestAt));

            foreach (var kv in _codex.ChaosConfigs.OrderBy(_ => Random.Shared.Next()))
            {
                var chaosConfig = kv.Value;
                var category = Enum.GetName(kv.Key);

                var currentSecond = DateTime.UtcNow.Second;

                if (currentSecond < 1)
                    KillsThisMinute = 0;

                var killRate = chaosConfig.KillRate * completionPercentage;
                var maxNumberOfPods = chaosConfig.MaxNumberOfPods;
                var memory = (long)(chaosConfig.Memory / completionPercentage);
                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

                await ChaosMonkey(killRate, currentSecond, maxNumberOfPods, category);

                await SetResourcesOnPods(category, nanoCpus, memory);
            }
        }
        else
        {
            _streamService.EndStreaming();
        }
        
        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }

    private async Task ChaosMonkey(double killRate, int currentSecond, double maxNumberOfPods, string? category)
    {
        var expectedKillCount = (int)(killRate / 60) * currentSecond;

        var overPodLimit = Math.Max(_readServiceService.GetAllPods().Count - maxNumberOfPods - expectedKillCount, 0);

        expectedKillCount += Convert.ToInt32(overPodLimit);

        for (var i = KillsThisMinute; i < expectedKillCount; i++)
        {
            var podsOfType = _readServiceService.GetAllPodTypes()
                .FindAll(podType => podType.ServiceTypes
                    .Any(serviceType => serviceType.ArtifactType.Equals(category)))
                .ToDictionary(p => p.Type);

            var podToKill = _readServiceService.GetAllPods()
                .Where(pod => podsOfType.ContainsKey(pod.Type))
                .MinBy(_ => Random.Shared.Next());

            if (podToKill is not null)
                await _resourceManagementService.RemovePodInstance(podToKill);
        }

        KillsThisMinute += expectedKillCount;
    }

    private async Task SetResourcesOnPods(string? category, long nanoCpus, long memory)
    {
        var podsOfTypeDict = _readServiceService.GetAllPodTypes()
            .FindAll(podType => podType.ServiceTypes
                .Any(serviceType => serviceType.ArtifactType.Equals(category)))
            .ToDictionary(p => p.Type);

        foreach (var pod in _readServiceService.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type)))
        {
            await _resourceManagementService.SetResources(pod, nanoCpus, memory);
        }
    }
}