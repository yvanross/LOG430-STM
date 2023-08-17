using Application.Mapping.Interfaces.Wrappers;
using System.Collections.Concurrent;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public class WrapperMediator : IDisposable
{
    internal ConcurrentDictionary<string, IStopWrapper> Stops { get; } = new();

    internal void AddStop(StopWrapper stop) => Stops.TryAdd(stop.Id, stop);

    public void Dispose()
    {
        Stops.Clear();
    }
}