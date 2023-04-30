using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Monitor = ApplicationLogic.Usecases.Monitor;

namespace Configuration;

public class SchedulingTask
{
    private readonly ILogger<SchedulingTask> _logger;

    public SchedulingTask(ILogger<SchedulingTask> logger)
    {
        _logger = logger;
        ScheduleRecurringTasks();
    }

    public void ScheduleRecurringTasks()
    {
        var readModel = new RepositoryRead();

        var monitor = new Monitor(readModel, new RepositoryWrite(), _logger);

        readModel.GetScheduler().TryAddTask(monitor.BeginProcessingHeartbeats);
    }
}