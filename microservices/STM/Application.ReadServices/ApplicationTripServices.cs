using System.Collections.Immutable;
using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Common.Interfaces;

namespace Application.QueryServices;

public class ApplicationTripServices
{
    private readonly ITripReadRepository _readTrips;
    private readonly IDatetimeProvider _datetimeProvider;

    public ApplicationTripServices(ITripReadRepository readTrips, IDatetimeProvider datetimeProvider)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
    }

    public ImmutableHashSet<Trip> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        var trips = _readTrips.GetTripsContainingStopsId(UniqueKeys(possibleSources, possibleDestinations));

        var relevantTrips = trips.Where(trip => trip.IsTimeRelevant(_datetimeProvider)).ToImmutableHashSet();

        return relevantTrips;

        IEnumerable<string> UniqueKeys(IEnumerable<Stop> sources, IEnumerable<Stop> destinations)
        {
            var uniqueKeys = new HashSet<string>(sources.Select(s=>s.Id));
            uniqueKeys.UnionWith(destinations.Select(s => s.Id));
            return uniqueKeys;
        }
    }
}