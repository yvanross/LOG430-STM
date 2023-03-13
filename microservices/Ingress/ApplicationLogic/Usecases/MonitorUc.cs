using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities.BusinessObjects;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IEnvironmentClient _client;

        private readonly IRepositoryRead _readModel;

        private readonly IRepositoryWrite _writeModel;

        private readonly ResourceManagementService _resourceManagementService;

        public MonitorUc(IEnvironmentClient client, IRepositoryRead readModel, IRepositoryWrite writeModel)
        {
            _client = client;
            _readModel = readModel;
            _writeModel = writeModel;

            _resourceManagementService = new ResourceManagementService(_client, readModel, writeModel);
        }

        public void TryScheduleHeartBeatOnScheduler()
        {
            if (_readModel.GetScheduler() is not { } scheduler) throw new NullReferenceException("Scheduler was null");

            scheduler.TryAddTask(BeginProcessingHeartbeats);
        }

        public void Acknowledge(Guid id)
        {
            var route = _readModel.ReadServiceById(id);

            if (route is not null)
            {
                route.LastHeartbeat = DateTime.UtcNow;
            }
        }

        private async Task BeginProcessingHeartbeats()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var routes = _readModel.GetAllServices();

                //if empty or null
                if ((routes?.Any() ?? true) is false) return Task.CompletedTask;

                var unknownStateRoutes = routes!.Where(service =>
                {
                    service.ServiceStatus?.EvaluateState();
                    return service.ServiceStatus is UnknownState;
                }).ToList();

                foreach (var route in unknownStateRoutes)
                {
                    var minNumberOfInstances = _readModel.GetServiceType(route.Type)!.MinimumNumberOfInstances;

                    if (_readModel.ReadServiceByType(route.Type)!.Count > minNumberOfInstances)
                    {
                        await _resourceManagementService.RemoveServiceInstance(route).ConfigureAwait(false);
                    }
                    else
                    {
                        await _resourceManagementService.ReplaceServiceInstance(route).ConfigureAwait(false);
                    }
                }

                return Task.CompletedTask;
            }, retryCount: int.MaxValue);
        }

        public async Task<ContainerInfo> GetPort(string containerId)
        {
            var runningServices = await _client.GetRunningServices();

            var targetService = runningServices.SingleOrDefault(rs => rs.Id.Equals(containerId));

            return targetService;
        }
    }
}
