using System.Collections.Immutable;
using System.Diagnostics;
using ApplicationLogic.Extensions;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services;

public class SchedulerService : IScheduler
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(100));

    private ImmutableDictionary<string, Func<Task>> _tasks = ImmutableDictionary<string, Func<Task>>.Empty;

    private ILogger _logger;

    public SchedulerService(ILogger<SchedulerService> logger)
    {
        _logger = logger;
        _ = BeginScheduling();
    }

    public void TryAddTask(string name, Func<Task> func)
    {
        ImmutableInterlocked.TryAdd(ref _tasks, func.Method.Name, func);

        _logger.LogInformation($"# Task: {name} has been scheduled #");
    }

    public void TryRemoveTask(string name)
    {
        ImmutableInterlocked.TryRemove(ref _tasks, name, out _);
    }

    private async Task BeginScheduling()
    {
        while (await _periodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            foreach (var task in _tasks)
            {
                await Try.WithConsequenceAsync(async () =>
                {
                    await task.Value();

                    return Task.CompletedTask;
                }, 
                    onFailure: (e, _) =>
                    {
                        _logger?.LogError(e.Message);
                        
                        _logger?.LogCritical(e.InnerException?.Message);
                        
                        _logger?.LogInformation(e.StackTrace);

                        return Task.CompletedTask;
                    },
                    autoThrow: false);
            }
        }
    }
}