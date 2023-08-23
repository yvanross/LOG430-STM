using Domain.Common.Interfaces;
using System.Collections.Immutable;
using Application.QueryServices.ProjectionModels;
using Application.QueryServices.ServiceInterfaces;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryServices;

public class ApplicationTripServices
{
    private readonly IQueryContext _readTrips;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationTripServices> _logger;

    public ApplicationTripServices(IQueryContext readTrips, IDatetimeProvider datetimeProvider, ILogger<ApplicationTripServices> logger)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    public async Task<ImmutableHashSet<Trip>> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        try
        {
            var materializedIds = UniqueKeys(possibleSources, possibleDestinations).ToList();

            var tripIds = await _readTrips
                .GetData<ScheduledStopProjection>()
                .Where(projection => materializedIds.Contains(projection.StopId))
                .Select(projection => projection.TripId)
                .Distinct()
                .ToListAsync();

            var trips = await _readTrips
                .GetData<Trip>()
                .Where(trip => tripIds.Contains(trip.Id))
                .ToListAsync();

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