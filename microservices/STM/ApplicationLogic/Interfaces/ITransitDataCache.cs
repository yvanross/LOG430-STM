using System.Collections.Concurrent;
using Domain.Aggregates;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Interfaces;

public interface ITransitDataCache
{
    ConcurrentDictionary<string, Trip>? GetTrips();
    
    ConcurrentDictionary<string, Stop>? GetStops();

    void AddStop(Stop stop);

    public Task PrefetchData();

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates);
}