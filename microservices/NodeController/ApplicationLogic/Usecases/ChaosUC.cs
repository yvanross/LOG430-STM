using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class ChaosUC
{
    private readonly ILogStore _logStore;

    private readonly IScheduler _scheduler;

    private readonly IRepositoryRead _readModel;

    private readonly ResourceManagementService _resourceManagementService;

    private int _killsThisMinute = 0;

    public ChaosUC(ILogStore logStore, IScheduler scheduler, IEnvironmentClient environmentClient, IRepositoryRead readModel, IRepositoryWrite writeModel)
    {
        _logStore = logStore;
        _scheduler = scheduler;
        _readModel = readModel;
        _resourceManagementService = new ResourceManagementService(environmentClient, readModel, writeModel);
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

                    var overPodLimit = Math.Max(_readModel.GetAllPods().Count - maxNumberOfPods - (expectedKillCount - _killsThisMinute), 0);

                    for (var i = _killsThisMinute; i < expectedKillCount + overPodLimit; i++)
                    {
                        var podsOfType = _readModel.GetAllPodTypes()
                            .FindAll(podType => podType.ServiceTypes
                                .Any(serviceType => serviceType.ComponentCategory.Equals(category)))
                            .ToDictionary(p => p.Type);

                        var podToKill = _readModel.GetAllPods().Where(pod => podsOfType.ContainsKey(pod.Type))
                            .MinBy(_ => Random.Shared.Next());

                        if (podToKill is not null)
                            await _resourceManagementService.RemovePodInstance(podToKill);
                    }

                    _killsThisMinute = expectedKillCount;

                    var podsOfTypeDict = _readModel.GetAllPodTypes()
                        .FindAll(podType => podType.ServiceTypes
                            .Any(serviceType => serviceType.ComponentCategory.Equals(category)))
                        .ToDictionary(p => p.Type);

                    foreach (var pod in _readModel.GetAllPods().Where(p => podsOfTypeDict.ContainsKey(p.Type)))
                    {
                        await _resourceManagementService.SetResources(pod, nanoCpus, memory);
                    }
                }

                _logStore.Log(new Snapshot()
                {
                    ServiceTypes = _readModel.GetAllServiceTypes().ToList(),
                    RunningInstances = _readModel.GetAllServices().ToList(),
                });
            }
        });

        double GetSecondsSinceEpoch(DateTime datetime) => (datetime - DateTime.UnixEpoch).TotalSeconds;
    }
                     
}