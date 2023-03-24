using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class ChaosExperimentUC
{
    private readonly IPodReadModel _readModelModel;
    
    private readonly ResourceManagementService _resourceManagementService;
    
    private readonly ChaosTestMonitoringService _chaosMonitoringService;

    private int _killsThisMinute;

    public ChaosExperimentUC(IEnvironmentClient environmentClient, IPodReadModel readModelModel, IPodWriteModel writeModelModel)
    {
        _readModelModel = readModelModel;
        _resourceManagementService = new ResourceManagementService(environmentClient, readModelModel, writeModelModel);
        _chaosMonitoringService = new ChaosTestMonitoringService();
    }

    public async Task InduceChaos(IChaosCodex codex, ISystemStateWriteModel systemStateWriteModel, IDataStreamReadModel streamReadModel)
    {
        _killsThisMinute = 0;

        while (DateTime.UtcNow > codex.EndTestAt)
        {
            var completionPercentage =
                (GetSecondsSinceEpoch(DateTime.UtcNow) - GetSecondsSinceEpoch(codex.StartTestAt)) /
                (GetSecondsSinceEpoch(codex.EndTestAt) - GetSecondsSinceEpoch(codex.StartTestAt));

            foreach (var kv in codex.ChaosConfigs.OrderBy(_ => Random.Shared.Next()))
            {
                var chaosConfig = kv.Value;
                var category = Enum.GetName(kv.Key);

                var currentSecond = DateTime.UtcNow.Second;

                if (currentSecond < 1 && _killsThisMinute > 1)
                    _killsThisMinute = 0;

                var killRate = chaosConfig.KillRate * completionPercentage;
                var maxNumberOfPods = chaosConfig.MaxNumberOfPods / completionPercentage;
                var memory = (long)(chaosConfig.Memory / completionPercentage);
                var nanoCpus = (long)(chaosConfig.NanoCpus / completionPercentage);

                var expectedKillCount = (int)(killRate / 60) * currentSecond;

                var overPodLimit = Math.Max(_readModelModel.GetAllPods().Count - maxNumberOfPods - (expectedKillCount - _killsThisMinute), 0);

                for (var i = _killsThisMinute; i < expectedKillCount + overPodLimit; i++)
                {
                    var podsOfType = _readModelModel.GetAllPodTypes()
                        .FindAll(podType => podType.ServiceTypes
                            .Any(serviceType => serviceType.ComponentCategory.Equals(category)))
                        .ToDictionary(p => p.Type);

                    var podToKill = _readModelModel.GetAllPods().Where(pod => podsOfType.ContainsKey(pod.Type))
                        .MinBy(_ => Random.Shared.Next());

                    if (podToKill is not null)
                        await _resourceManagementService.RemovePodInstance(podToKill);
                }

                _killsThisMinute = expectedKillCount;

                var podsOfTypeDict = _readModelModel.GetAllPodTypes()
                    .FindAll(podType => podType.ServiceTypes
                        .Any(serviceType => serviceType.ComponentCategory.Equals(category)))
                    .ToDictionary(p => p.Type);

                foreach (var pod in _readModelModel.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type)))
                {
                    await _resourceManagementService.SetResources(pod, nanoCpus, memory);
                }
            }

            streamReadModel.BeginStreaming();

            await systemStateWriteModel.Log(new ExperimentReport()
            {
                ServiceTypes = _readModelModel.GetAllServiceTypes().ToList(),
                RunningInstances = _readModelModel.GetAllServices().ToList(),
                ExperimentResult = _chaosMonitoringService.ExperimentResult
            });
        }

        await streamReadModel.EndStreaming();

        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }

    public Task ReportTestResult(ISaga saga)
    {
        return _chaosMonitoringService.AnalyzeAndStoreRealtimeTestData(saga);
    }
}