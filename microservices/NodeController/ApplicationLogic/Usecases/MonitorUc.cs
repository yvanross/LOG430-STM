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
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IRepositoryRead _readModel;

        private readonly ResourceManagementService _resourceManagementService;

        public MonitorUc(IEnvironmentClient client, IRepositoryRead readModel, IRepositoryWrite writeModel)
        {
            _readModel = readModel;

            _resourceManagementService = new ResourceManagementService(client, readModel, writeModel);
        }

        public void TryScheduleStateProcessingOnScheduler()
        {
            if (_readModel.GetScheduler() is not { } scheduler) throw new NullReferenceException("Scheduler was null");

            scheduler.TryAddTask(BeginProcessingPodStates);
        }

        private async Task BeginProcessingPodStates()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var routes = _readModel.GetAllPods();

                //if empty or null
                if ((routes?.Any() ?? true) is false) return Task.CompletedTask;

                var unknownPodStates = routes!.Where(pod =>
                {
                    pod.ServiceInstances.ForEach(service => service.ServiceStatus?.EvaluateState());

                    return pod.ServiceInstances.Any(service => service.ServiceStatus is UnknownState);
                }).ToList();

                foreach (var podInstance in unknownPodStates)
                {
                    var minNumberOfInstances = _readModel.GetPodType(podInstance.Type)!.MinimumNumberOfInstances;

                    if (_readModel.GetServiceInstances(podInstance.Type)!.Count > minNumberOfInstances)
                    {
                        await _resourceManagementService.RemovePodInstance(podInstance).ConfigureAwait(false);
                    }
                    else
                    {
                        await _resourceManagementService.ReplacePodInstance(podInstance).ConfigureAwait(false);
                    }
                }

                return Task.CompletedTask;
            }, retryCount: 5);
        }
    }
}
