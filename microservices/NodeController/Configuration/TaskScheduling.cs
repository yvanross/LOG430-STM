using ApplicationLogic.Interfaces;
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
    private readonly IHostInfo _hostInfo;

    public TaskScheduling(ILogger<TaskScheduling> logger, IScheduler scheduler, Ingress ingress, ServicePoolDiscovery servicePool, Monitor monitor, IHostInfo hostInfo)
    {
        _logger = logger;
        _scheduler = scheduler;
        _ingress = ingress;
        _servicePool = servicePool;
        _monitor = monitor;
        _hostInfo = hostInfo;
    }

    public async Task ScheduleRecurringTasks()
    {
        _logger.LogInformation("# Schedule Recurring Tasks #");

        if (_hostInfo.IsIngressConfigValid())
        {
            await _ingress.Register();

            _scheduler.TryAddTask(nameof(_ingress.HeartBeat), _ingress.HeartBeat);
        }

        _scheduler.TryAddBlockingTask(nameof(_servicePool.DiscoverServices), _servicePool.DiscoverServices);

        _scheduler.TryAddBlockingTask(nameof(_monitor.RemoveOrReplaceDeadPodsFromModel), _monitor.RemoveOrReplaceDeadPodsFromModel);
        _scheduler.TryAddBlockingTask(nameof(_monitor.MatchInstanceDemandOnPods), _monitor.MatchInstanceDemandOnPods);
    }
}