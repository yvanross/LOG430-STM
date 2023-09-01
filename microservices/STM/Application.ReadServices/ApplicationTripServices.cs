using Domain.Common.Interfaces;
using System.Collections.Immutable;
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

            List<string> tripIds;

            if (_readTrips.IsInMemory() is false)
            {
                tripIds = await _readTrips
                    .GetData<ScheduledStop>()
                    .Where(scheduledStop => materializedIds.Contains(scheduledStop.StopId))
                    .Select(scheduledStop => EF.Property<string>(scheduledStop, "TripId"))
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                var allTrips = _readTrips
                    .GetData<Trip>()
                    //.Include(x => x.ScheduledStops)
                    .ToList();

                var materializedHashIds = materializedIds.ToHashSet();

                tripIds = allTrips.AsParallel().Where(trip =>
                        trip.ScheduledStops.Exists(scheduledStop => materializedHashIds.Contains(scheduledStop.StopId)))
                    .Select(trip => trip.Id)
                    .Distinct()
                    .ToList();
            }

            var trips = await _readTrips
                .GetData<Trip>()
                .Where(trip => tripIds.Contains(trip.Id))
                .Include(trip => trip.ScheduledStops)
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