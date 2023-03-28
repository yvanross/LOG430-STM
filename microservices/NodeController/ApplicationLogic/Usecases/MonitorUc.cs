using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IPodReadModel _readModelModel;

        private readonly ResourceManagementService _resourceManagementService;

        private readonly IEnvironmentClient _client;

        public MonitorUc(IEnvironmentClient client, IPodReadModel readModelModel, IPodWriteModel writeModelModel)
        {
            _readModelModel = readModelModel;

            _client = client;

            _resourceManagementService = new ResourceManagementService(client, readModelModel, writeModelModel);
        }

        public async Task GarbageCollection()
        {
            await _client.GarbageCollection();
        }

        public async Task MatchInstanceDemandOnPods()
        {
            var podTypes = _readModelModel.GetAllPodTypes();

            foreach (var podType in podTypes)
            {
                var podCount = _readModelModel.GetPodInstances(podType.Type)?.Count;

                if (podCount < podType.MinimumNumberOfInstances)
                {
                    await _resourceManagementService.IncreaseNumberOfPodInstances(podType.Type).ConfigureAwait(false);
                }
            }
        }

        public async Task RemoveOrReplaceDeadPodsFromModel()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var runningContainerIds = (await _client.GetRunningServices())?.ToHashSet();

                if (runningContainerIds is null) return Task.CompletedTask;

                var allPods = _readModelModel.GetAllPods();

                foreach (var podInstance in allPods)
                {
                    var minNumberOfInstances = _readModelModel.GetPodType(podInstance.Type)!.MinimumNumberOfInstances;

                    if (IsAnyPodServiceDown(podInstance, runningContainerIds))
                    {
                        if (IsNumberOfRunningInstancesGreaterThanRequired(podInstance, minNumberOfInstances))
                        {
                            await _resourceManagementService.RemovePodInstance(podInstance).ConfigureAwait(false);
                        }
                        else
                        {
                            await _resourceManagementService.ReplacePodInstance(podInstance).ConfigureAwait(false);
                        }
                    }
                }

                return Task.CompletedTask;
            }, retryCount: 2).ConfigureAwait(false);

            bool IsAnyPodServiceDown(IPodInstance podInstance, HashSet<string> runningContainerIds)
            {
                return podInstance.ServiceInstances.Any(serviceInstance => runningContainerIds.Contains(serviceInstance.ContainerInfo?.Id ?? string.Empty)) is false;
            }

            bool IsNumberOfRunningInstancesGreaterThanRequired(IPodInstance podInstance, int minNumberOfInstances)
            {
                var count = _readModelModel.GetServiceInstances(podInstance.Type)!.Count;

                return count > minNumberOfInstances;
            }
        }
    }
}
