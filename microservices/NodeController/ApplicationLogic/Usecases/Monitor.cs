using ApplicationLogic.Services;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.Extensions;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class Monitor
    {
        private readonly IPodReadService _readServiceService;

        private readonly ILogger<Monitor> _logger;

        private readonly ResourceManagementService _resourceManagementService;

        public Monitor(IPodReadService readServiceService, ResourceManagementService resourceManagementService, ILogger<Monitor> logger)
        {
            _readServiceService = readServiceService;
            _logger = logger;
            _resourceManagementService = resourceManagementService;
        }

        public async Task MatchInstanceDemandOnPods()
        {
            await CreateServicesForIncompleteNewPods();

            await IncreasePodNumberToMatchDemand();

            async Task CreateServicesForIncompleteNewPods()
            {
                var podsInLaunchedState =
                    _readServiceService.GetAllPods().Where(pod => pod.ServiceStatus is LaunchedState).ToList();

                foreach (var podInstance in podsInLaunchedState)
                {
                    var podType = _readServiceService.GetPodType(podInstance.Type)!;

                    var serviceTypesNotInCollection = podType.ServiceTypes
                        .WhereNotTrueInAnyOf(podInstance.ServiceInstances,
                            (serviceType, serviceInstance) => serviceType.Type.EqualsIgnoreCase(serviceInstance.Type));

                    foreach (var serviceType in serviceTypesNotInCollection)
                    {
                        await _resourceManagementService.CreateNewSidecarInstanceOnPod(serviceType.Type, podType.Type, podInstance.Id).ConfigureAwait(false);
                    }
                }
            }

            async Task IncreasePodNumberToMatchDemand()
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
        }

        public async Task RemoveOrReplaceDeadPodsFromModel()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var runningContainerIds = (await _resourceManagementService.GetRunningServices())?.ToHashSet();

                if (runningContainerIds is null) return Task.CompletedTask;

                var allPods = _readServiceService.GetAllPods();

                foreach (var podInstance in allPods)
                {
                    var minNumberOfInstances = _readServiceService.GetPodType(podInstance.Type)!.NumberOfInstances;

                    if (IsAnyPodServiceDownOnInstance(podInstance, runningContainerIds))
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

                    if (DidPodLoseAnyServiceInstance(podInstance))
                    {
                        await _resourceManagementService.RemovePodInstance(podInstance).ConfigureAwait(false);
                        
                        await _resourceManagementService.IncreaseNumberOfPodInstances(podInstance.Type).ConfigureAwait(false);
                    }
                }

                return Task.CompletedTask;
            }, retryCount: 2).ConfigureAwait(false);

            bool DidPodLoseAnyServiceInstance(IPodInstance podInstance)
            {
                if(podInstance.ServiceStatus is LaunchedState) return false;

                var podType = _readServiceService.GetPodType(podInstance.Type)!;

                return podType.ServiceTypes
                    .All(serviceType => podInstance.ServiceInstances.
                        Any(serviceInstance => serviceInstance.Type.Equals(serviceType.Type))) is false;
            }

            bool IsAnyPodServiceDownOnInstance(IPodInstance podInstance, HashSet<string> runningContainerIds)
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
