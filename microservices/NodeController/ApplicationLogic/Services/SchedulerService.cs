using System.Collections.Immutable;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services;

public class SchedulerService : IScheduler
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(500));

    private readonly PeriodicTimer _periodicTimerBlocking = new(TimeSpan.FromMilliseconds(500));

    private ImmutableList<(string name, Func<Task> func)> _tasks = ImmutableList<(string name, Func<Task>)>.Empty;

    private ImmutableList<(string name, Func<Task> func)> _blockingTasks = ImmutableList<(string name, Func<Task>)>.Empty;

    private readonly ILogger _logger;

    private readonly Mutex _mutex = new();

    private readonly Mutex _mutexBlocking = new();

    public SchedulerService(ILogger<SchedulerService> logger)
    {
        _logger = logger;
        _ = BeginScheduling();
        _ = BeginSchedulingBlocking();
    }

    public void TryAddTask(string name, Func<Task> func)
    {
        _mutex.WaitOne();

        if (_tasks.Any(x => x.name.Equals(name)) is false)
        {
            _logger.LogInformation($"# Task: {name} has been scheduled #");
            _tasks = _tasks.Add((name, func));
        }

        _mutex.ReleaseMutex();
    }

    public void TryAddBlockingTask(string name, Func<Task> func)
    {
        _mutexBlocking.WaitOne();

        if (_blockingTasks.Any(x => x.name.Equals(name)) is false)
        {
            _logger.LogInformation($"# Task: {name} has been scheduled #");
            _blockingTasks = _blockingTasks.Add((name, func));
        }

        _mutexBlocking.ReleaseMutex();
    }

    public void TryRemoveTask(string name)
    {
        ImmutableInterlocked.Update(ref _tasks, 
            (collection) => collection.RemoveAll(t=>t.name.Equals(name)));
    }

    public void TryRemoveBlockingTask(string name)
    {
        ImmutableInterlocked.Update(ref _blockingTasks,
            (collection) => collection.RemoveAll(t => t.name.Equals(name)));
    }

    private async Task BeginScheduling()
    {
        while (await _periodicTimer.WaitForNextTickAsync())
        {
            foreach (var task in _tasks)
            {
                try
                {
                    _ = task.func();
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.Message);

                    _logger?.LogCritical(e.InnerException?.Message);

                    _logger?.LogInformation(e.StackTrace);
                }
            }
        }
    }

    private async Task BeginSchedulingBlocking()
    {
        while (await _periodicTimerBlocking.WaitForNextTickAsync())
        {
            foreach (var task in _blockingTasks)
            {
                await Try.WithConsequenceAsync(async () =>
                    {
                        await task.func();

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