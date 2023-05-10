using System.Collections.Concurrent;
using Entities.Common.Interfaces;
using Entities.Gtfs.Interfaces;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Interfaces;

public interface ITransitDataCache
{
    ConcurrentDictionary<string, ITrip>? GetTrips();
    
    ConcurrentDictionary<string, IStop>? GetStops();

    void AddStop(IStop stop);

    public Task PrefetchData();

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates);
}