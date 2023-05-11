using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class Monitor
    {
        private readonly IPodReadService _readServiceService;

        private readonly ILogger<Monitor> _logger;

        private readonly ResourceManagementService _resourceManagementService;

        private readonly IEnvironmentClient _client;

        public Monitor(IEnvironmentClient client, IPodReadService readServiceService, IPodWriteService writeServiceService, ILogger<Monitor> logger)
        {
            _readServiceService = readServiceService;
            _logger = logger;
            _client = client;
            _resourceManagementService = new ResourceManagementService(client, readServiceService, writeServiceService);
        }

        public async Task GarbageCollection()
        {
            await _client.GarbageCollection();
        }

        public async Task MatchInstanceDemandOnPods()
        {
            var podTypes = _readServiceService.GetAllPodTypes();

            foreach (var podType in podTypes)
            {
                var podCount = _readServiceService.GetPodInstances(podType.Type)?.Count;

                if (podCount < podType.NumberOfInstances)
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

                var allPods = _readServiceService.GetAllPods();

                foreach (var podInstance in allPods)
                {
                    var minNumberOfInstances = _readServiceService.GetPodType(podInstance.Type)!.NumberOfInstances;

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

                    if (PodServicesAllNecessaryInstances(podInstance) is false)
                    {
                        await _resourceManagementService.RemovePodInstance(podInstance).ConfigureAwait(false);
                        
                        await _resourceManagementService.IncreaseNumberOfPodInstances(podInstance.Type).ConfigureAwait(false);
                    }
                }

                return Task.CompletedTask;
            }, retryCount: 2).ConfigureAwait(false);

            bool PodServicesAllNecessaryInstances(IPodInstance podInstance)
            {
                var podType = _readServiceService.GetPodType(podInstance.Type)!;

                return podType.ServiceTypes
                    .All(serviceType => podInstance.ServiceInstances.
                        Any(serviceInstance => serviceInstance.Type.Equals(serviceType.Type)));
            }

            bool IsAnyPodServiceDown(IPodInstance podInstance, HashSet<string> runningContainerIds)
            {
                var anyServiceDown = podInstance.ServiceInstances.Any(serviceInstance =>
                    runningContainerIds.Contains(serviceInstance.ContainerInfo?.Id ?? string.Empty) is false);

                return anyServiceDown;
            }

            bool IsNumberOfRunningInstancesGreaterThanRequired(IPodInstance podInstance, int minNumberOfInstances)
            {
                var count = _readServiceService.GetPodInstances(podInstance.Type)!.Count;

                return count > minNumberOfInstances;
            }
        }
    }
}
