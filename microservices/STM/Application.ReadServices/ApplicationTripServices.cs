using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Common.Interfaces;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationTripServices
{
    private readonly ITripReadRepository _readTrips;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationTripServices> _logger;

    public ApplicationTripServices(ITripReadRepository readTrips, IDatetimeProvider datetimeProvider, ILogger<ApplicationTripServices> logger)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    public ImmutableHashSet<Trip> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        try
        {
            var trips = _readTrips.GetTripsContainingStopsId(UniqueKeys(possibleSources, possibleDestinations));

            var relevantTrips = trips.Where(trip => trip.IsTimeRelevant(_datetimeProvider)).ToImmutableHashSet();

            return relevantTrips;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting time relevant trips containing source and destination");

            throw;
        }
        
        IEnumerable<string> UniqueKeys(IEnumerable<Stop> sources, IEnumerable<Stop> destinations)
        {
            var uniqueKeys = new HashSet<string>(sources.Select(s => s.Id));
            uniqueKeys.UnionWith(destinations.Select(s => s.Id));
            return uniqueKeys;
        }
    }
}