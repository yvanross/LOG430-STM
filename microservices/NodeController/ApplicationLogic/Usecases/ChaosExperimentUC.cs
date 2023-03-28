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

    public IExperimentResult? ExperimentResult => _chaosMonitoringService.ExperimentResult;

    private readonly IPodReadModel _readModelModel;

    private readonly IChaosCodex _codex;
  
    private readonly ISystemStateWriteModel _systemStateWriteModel;
    
    private readonly IDataStreamReadModel _streamReadModel;

    private readonly ResourceManagementService _resourceManagementService;
    
    private readonly ChaosTestMonitoringService _chaosMonitoringService;

    public ChaosExperimentUC(IEnvironmentClient environmentClient, IPodReadModel readModelModel, IPodWriteModel writeModelModel,
        IChaosCodex codex, ISystemStateWriteModel systemStateWriteModel, IDataStreamReadModel streamReadModel)
    {
        _readModelModel = readModelModel;
        _codex = codex;
        _systemStateWriteModel = systemStateWriteModel;
        _streamReadModel = streamReadModel;
        _resourceManagementService = new ResourceManagementService(environmentClient, readModelModel, writeModelModel);
        _chaosMonitoringService = new ChaosTestMonitoringService();
    }

    public async Task InduceChaos()
    {
        if (DateTime.UtcNow < _codex.EndTestAt)
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
                var maxNumberOfPods = chaosConfig.MaxNumberOfPods / completionPercentage;
                var memory = (long)(chaosConfig.Memory / completionPercentage);
                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

                await ChaosMonkey(killRate, currentSecond, maxNumberOfPods, category);

                await SetResourcesOnPods(category, nanoCpus, memory);
            }

            await StreamAndLogExperimentResults();
        }
        else
        {
            _streamReadModel.EndStreaming();

            //todo clean up resources after experiment and refactoring this ugly method
        }
        
        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }

    private async Task ChaosMonkey(double killRate, int currentSecond, double maxNumberOfPods, string? category)
    {
        var expectedKillCount = (int)(killRate / 60) * currentSecond;

        var overPodLimit = Math.Max(_readModelModel.GetAllPods().Count - maxNumberOfPods - expectedKillCount, 0);

        expectedKillCount += Convert.ToInt32(overPodLimit);

        for (var i = KillsThisMinute; i < expectedKillCount; i++)
        {
            var podsOfType = _readModelModel.GetAllPodTypes()
                .FindAll(podType => podType.ServiceTypes
                    .Any(serviceType => serviceType.ArtifactType.Equals(category)))
                .ToDictionary(p => p.Type);

            var podToKill = _readModelModel.GetAllPods()
                .Where(pod => podsOfType.ContainsKey(pod.Type))
                .MinBy(_ => Random.Shared.Next());

            if (podToKill is not null)
                await _resourceManagementService.RemovePodInstance(podToKill);
        }

        KillsThisMinute += expectedKillCount;
    }

    private async Task SetResourcesOnPods(string? category, long nanoCpus, long memory)
    {
        var podsOfTypeDict = _readModelModel.GetAllPodTypes()
            .FindAll(podType => podType.ServiceTypes
                .Any(serviceType => serviceType.ArtifactType.Equals(category)))
            .ToDictionary(p => p.Type);

        foreach (var pod in _readModelModel.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type)))
        {
            await _resourceManagementService.SetResources(pod, nanoCpus, memory);
        }
    }

    private async Task StreamAndLogExperimentResults()
    {
        _streamReadModel.BeginStreaming(ReportTestResult);

        await _systemStateWriteModel.Log(new ExperimentReport()
        {
            ServiceTypes = _readModelModel.GetAllServiceTypes().ToList(),
            RunningInstances = _readModelModel.GetAllServices().ToList(),
            ExperimentResult = _chaosMonitoringService.ExperimentResult
        });
    }

    public void ReportTestResult(ISaga saga)
    {
        _chaosMonitoringService.AnalyzeAndStoreRealtimeTestData(saga);
    }
}