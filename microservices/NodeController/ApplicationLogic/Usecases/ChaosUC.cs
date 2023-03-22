using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class ChaosUC
{
    private readonly ILogStoreWriteModel _logStoreWriteModel;

    private readonly IScheduler _scheduler;

    private readonly IPodReadModel _readModelModel;
    
    private readonly IDataStreamReadModel _streamReadModel;

    private readonly ResourceManagementService _resourceManagementService;

    private int _killsThisMinute = 0;

    public ChaosUC(ILogStoreWriteModel logStoreWriteModel, IScheduler scheduler, IEnvironmentClient environmentClient,
                    IPodReadModel readModelModel, IPodWriteModel writeModelModel, IDataStreamReadModel streamReadModel)
    {
        _logStoreWriteModel = logStoreWriteModel;
        _scheduler = scheduler;
        _readModelModel = readModelModel;
        _streamReadModel = streamReadModel;
        _resourceManagementService = new ResourceManagementService(environmentClient, readModelModel, writeModelModel);
    }

    public void InduceChaos(IChaosCodex codex)
    {
        _scheduler.TryAddTask("chaosTest", async () =>
        {
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

                var sagas = await _streamReadModel.BeginStreaming();

                await _logStoreWriteModel.Log(new Snapshot()
                {
                    ServiceTypes = _readModelModel.GetAllServiceTypes().ToList(),
                    RunningInstances = _readModelModel.GetAllServices().ToList(),
                    Saga = sagas
                });
            }
        });

        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }
                     
}