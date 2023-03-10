using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Services;

public class HeartBeatService
{
    private readonly IRepositoryRead _readModel;
    
    private readonly IRepositoryWrite _writeModel;

    private readonly IEnvironmentClient _environmentClient;

    private readonly ConcurrentQueue<Guid> _potentiallyDeadServices = new ();

    public HeartBeatService(IRepositoryRead readModel, IRepositoryWrite writeModel, IEnvironmentClient environmentClient)
    {
        _readModel = readModel;
        _writeModel = writeModel;
        _environmentClient = environmentClient;

        if (readModel.GetScheduler() is not { } scheduler) throw new NullReferenceException("Scheduler was null");

        scheduler.TryAddTask(BeginProcessingHeartbeats);
    }

    public void Acknowledge(Guid id)
    {
        var route = _readModel.ReadServiceById(id);

        if (route is not null)
        {
            route.LastHeartbeat = DateTime.UtcNow;

            _writeModel.WriteService(route);
        }
    }

    private async Task BeginProcessingHeartbeats()
    {
        await Try.WithConsequenceAsync(() =>
        {
            var routes = _readModel.GetAllServices();

            while (_potentiallyDeadServices.IsEmpty is false)
            {
                _potentiallyDeadServices.TryDequeue(out var idToAudit);

                var serviceToAudit = _readModel.ReadServiceById(idToAudit);

                if (serviceToAudit is not null && serviceToAudit.LastHeartbeat >= DateTime.UtcNow.AddSeconds(-1))
                {
                    _writeModel.RemoveService(serviceToAudit);

                    _ = _environmentClient.RemoveContainerInstance(serviceToAudit.ContainerInfo.Id);

                    var serviceType = _readModel.GetServiceType(serviceToAudit.Type);

                    if (serviceType is null) continue;

                    _ = _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{serviceToAudit.ContainerInfo.Name}_{Guid.NewGuid()}");
                }
            }

            routes?.Where(r => r.LastHeartbeat <= DateTime.UtcNow.AddSeconds(-0.5)).ToList().ForEach(service => _potentiallyDeadServices.Enqueue(service.Id));

            return Task.FromResult(Task.FromResult(0));
        }, retryCount: int.MaxValue);
    }
}