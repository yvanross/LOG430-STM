using Domain.Aggregates;
using Domain.Common.Interfaces;

namespace Domain.Services;

public class TripServices
{
    private readonly IDatetimeProvider _datetimeProvider;

    public TripServices(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    public Dictionary<string, Trip> TimeRelevantTripsContainingSourceAndDestination(
        IEnumerable<Trip> trips, 
        Dictionary<string, Stop> possibleSources,
        Dictionary<string, Stop> possibleDestinations)
    {
        var relevantTrips = trips
            .Where(trip =>
                trip.IsTimeRelevant(_datetimeProvider) &&
                possibleSources.Any(source => trip.ContainsStop(source.Key)) &&
                possibleDestinations.Any(destination => trip.ContainsStop(destination.Key)))
            .ToDictionary(trip => trip.Id);

        return relevantTrips;
    }
}