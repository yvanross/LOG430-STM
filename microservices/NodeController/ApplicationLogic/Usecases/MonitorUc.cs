using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IRepositoryRead _readModel;

        private readonly ResourceManagementService _resourceManagementService;

        private readonly IEnvironmentClient _client;

        public MonitorUc(IEnvironmentClient client, IRepositoryRead readModel, IRepositoryWrite writeModel)
        {
            _readModel = readModel;

            _client = client;

            _resourceManagementService = new ResourceManagementService(client, readModel, writeModel);
        }

        public void TryScheduleStateProcessingOnScheduler()
        {
            if (_readModel.GetScheduler() is not { } scheduler) throw new NullReferenceException("Scheduler was null");

            scheduler.TryAddTask(nameof(BeginProcessingPodStates), BeginProcessingPodStates);
            
            scheduler.TryAddTask(nameof(GarbageCollection), GarbageCollection);
        }

        private async Task GarbageCollection()
        {
            await _client.GarbageCollection();
        }

        private async Task BeginProcessingPodStates()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var runningContainerIds = (await _client.GetRunningServices())?.ToHashSet();

                if (runningContainerIds is null) return Task.CompletedTask;

                var routes = _readModel.GetAllPods();

                foreach (var podInstance in routes)
                {
                    var minNumberOfInstances = _readModel.GetPodType(podInstance.Type)!.MinimumNumberOfInstances;

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
            }, retryCount: 5);

            bool IsAnyPodServiceDown(IPodInstance podInstance, HashSet<string> runningContainerIds)
            {
                return podInstance.ServiceInstances.Any(serviceInstance => runningContainerIds.Contains(serviceInstance.ContainerInfo?.Id ?? string.Empty)) is false;
            }

            bool IsNumberOfRunningInstancesGreaterThanRequired(IPodInstance podInstance, int minNumberOfInstances)
            {
                return _readModel.GetServiceInstances(podInstance.Type)!.Count > minNumberOfInstances;
            }
        }
    }
}
