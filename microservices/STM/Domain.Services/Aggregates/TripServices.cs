using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Domain.Common.Interfaces;
using Domain.Factories;

namespace Domain.Services.Aggregates;

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

    public Trip CreateTrip(string tripId, IEnumerable<(string stopId, DateTime schedule)> stopSchedules)
    {
        return TripFactory.Create(tripId, stopSchedules);
    }
}