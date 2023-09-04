using System.Collections.Concurrent;
using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public class WrapperMediator : IDisposable
{
    internal ConcurrentDictionary<string, IStopWrapper> Stops { get; } = new();

    public void Dispose()
    {
        Stops.Clear();
    }

    internal void AddStop(StopWrapper stop)
    {
        Stops.TryAdd(stop.Id, stop);
    }
}