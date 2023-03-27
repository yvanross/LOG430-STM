using System.Collections.Immutable;
using ApplicationLogic.Extensions;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services;

public class SchedulerService : IScheduler
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(100));

    private ImmutableDictionary<string, Func<Task>> _tasks = ImmutableDictionary<string, Func<Task>>.Empty;

    private ILogger? _logger;

    public SchedulerService()
    {
        _ = BeginScheduling();
    }

    public void TryAddTask(string name, Func<Task> func)
    {
        ImmutableInterlocked.TryAdd(ref _tasks, func.Method.Name, func);
    }

    public void TryRemoveTask(string name)
    {
        ImmutableInterlocked.TryRemove(ref _tasks, name, out _);
    }

    public void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    private async Task BeginScheduling()
    {
        while (await _periodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            foreach (var func in _tasks.Select(kv=>kv.Value))
            {
                await Try.WithConsequenceAsync(async () =>
                {
                    await func();

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