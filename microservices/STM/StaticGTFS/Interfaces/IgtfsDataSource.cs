using Entities.Domain;
using STM.ExternalServiceProvider.Proto;
using System.Collections.Immutable;

namespace GTFS.Interfaces;

public interface IgtfsDataSource
{
    ImmutableDictionary<string, IGTFSTrip>? GetTrips();
    
    ImmutableDictionary<string, IStopSTM>? GetStops();

    void AddStop(IStopSTM igtfsStop);

    public Task PrefetchData();

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates);
}