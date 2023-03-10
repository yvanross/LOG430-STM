using System.Collections.Immutable;
using ApplicationLogic.Extensions;
using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class Scheduler : IScheduler
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(0.5));

    private ImmutableDictionary<string, Func<Task>> _tasks = ImmutableDictionary<string, Func<Task>>.Empty;

    public Scheduler()
    {
        _ = BeginScheduling();
    }

    public void TryAddTask(Func<Task> func)
    {
        if (_tasks.ContainsKey(func.Method.Name) is false)
        {
            _tasks = _tasks.Add(func.Method.Name, func);
        }
    }

    private async Task BeginScheduling()
    {
        while (await _periodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            foreach (var func in _tasks.Select(kv=>kv.Value))
            {
                await Try.WithConsequenceAsync(() =>
                {
                    _ = func();

                    return Task.FromResult(Task.FromResult(0));
                }, retryCount: int.MaxValue);
            }
        }
    }
}