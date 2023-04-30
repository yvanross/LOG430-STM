using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using Infrastructure.Dao;
using Infrastructure.Docker;
using Infrastructure.Ingress;
using Microsoft.Extensions.Logging;
using Monitor = ApplicationLogic.Usecases.Monitor;

namespace Configuration;

public class TaskScheduling
{
    private readonly ILogger _logger;
    private readonly IScheduler _scheduler;
    private readonly Ingress _ingress;
    private readonly ServicePoolDiscovery _servicePool;
    private readonly Monitor _monitor;

    public TaskScheduling(ILogger<TaskScheduling> logger, IScheduler scheduler, Ingress ingress, ServicePoolDiscovery servicePool, Monitor monitor)
    {
        _logger = logger;
        _scheduler = scheduler;
        _ingress = ingress;
        _servicePool = servicePool;
        _monitor = monitor;
    }

    public void ScheduleRecurringTasks()
    {
        _logger.LogInformation("# Schedule Recurring Tasks #");

        _ingress.Register().Wait();

        _ingress.GetLogStoreAddressAndPort().Wait();

        _logger.LogInformation("# Preparation Complete, scheduling... #");

        _scheduler.TryAddTask(nameof(_servicePool.DiscoverServices), _servicePool.DiscoverServices);
        //readModel.GetScheduler().TryAddTask(nameof(monitor.RemoveOrReplaceDeadPodsFromModel), monitor.RemoveOrReplaceDeadPodsFromModel);
        //_scheduler.TryAddTask(nameof(_monitor.MatchInstanceDemandOnPods), _monitor.MatchInstanceDemandOnPods);
        //readModel.GetScheduler().TryAddTask(nameof(monitor.GarbageCollection), monitor.GarbageCollection);
    }
}