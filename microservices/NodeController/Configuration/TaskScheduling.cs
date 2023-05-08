using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
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

        _ = _ingress.Register();

        _logger.LogInformation("# Preparation Complete, scheduling... #");

        _scheduler.TryAddTask(nameof(_servicePool.DiscoverServices), _servicePool.DiscoverServices);
        _scheduler.TryAddTask(nameof(_monitor.RemoveOrReplaceDeadPodsFromModel), _monitor.RemoveOrReplaceDeadPodsFromModel);
        _scheduler.TryAddTask(nameof(_monitor.MatchInstanceDemandOnPods), _monitor.MatchInstanceDemandOnPods);
        _scheduler.TryAddTask(nameof(_ingress.HeartBeat), _ingress.HeartBeat);
    }
}