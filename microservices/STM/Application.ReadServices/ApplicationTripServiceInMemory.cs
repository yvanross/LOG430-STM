using Domain.Common.Interfaces;
using System.Collections.Immutable;
using Application.QueryServices.ServiceInterfaces;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryServices;

public class ApplicationTripServiceInMemory : IApplicationTripService
{
    private readonly IQueryContext _readTrips;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationTripServiceInMemory> _logger;

    public ApplicationTripServiceInMemory(IQueryContext readTrips, IDatetimeProvider datetimeProvider, ILogger<ApplicationTripServiceInMemory> logger)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    //this code is a bit much, but for the needs of the lab it needs to work both for "in memory" and for the database with relative efficiency
    public async Task<ImmutableHashSet<Trip>> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        try
        {
            var materializedIds = GetUniqueStopIds(possibleSources, possibleDestinations);

            var tripProjection = await GetRelevantTripProjectionAsync(materializedIds, _datetimeProvider.GetCurrentTime());

            var trips = await GetTripsByTripIdsAsync(tripProjection.Keys);

            AttachScheduledStopsToTrips(trips, tripProjection);

            return trips.ToImmutableHashSet();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting time relevant trips containing source and destination");
            throw;
        }
    }

    private List<string> GetUniqueStopIds(IEnumerable<Stop> sources, IEnumerable<Stop> destinations)
    {
        var uniqueKeys = new HashSet<string>(sources.Select(s => s.Id));

        uniqueKeys.UnionWith(destinations.Select(s => s.Id));

        return uniqueKeys.ToList();
    }

    private async Task<Dictionary<string, List<ScheduledStop>>> GetRelevantTripProjectionAsync(List<string> materializedIds, DateTime currentTime)
    {
        var tripsIdsAndScheduledStops = await _readTrips
            .GetData<ScheduledStop>()
            .Where(scheduledStop => materializedIds.Contains(scheduledStop.StopId) &&
                                    scheduledStop.DepartureTime > currentTime)
            .Select(scheduledStop => new
            {
                TripId = EF.Property<string>(scheduledStop, "TripId"),
                ScheduledStop = scheduledStop,
            })
            .ToListAsync();

        return tripsIdsAndScheduledStops
            .GroupBy(tuple => tuple.TripId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(tuple => tuple.ScheduledStop)
                             .OrderBy(scheduledStop => scheduledStop.DepartureTime)
                             .ToList());
    }

    private async Task<List<Trip>> GetTripsByTripIdsAsync(IEnumerable<string> tripIds)
    {
        return await _readTrips
            .GetData<Trip>()
            .Where(trip => tripIds.Contains(trip.Id))
            .ToListAsync();
    }

    private void AttachScheduledStopsToTrips(List<Trip> trips, Dictionary<string, List<ScheduledStop>> tripProjection)
    {
        foreach (var trip in trips)
        {
            trip.ScheduledStops = new List<ScheduledStop>(tripProjection[trip.Id]);
        }
    }
}