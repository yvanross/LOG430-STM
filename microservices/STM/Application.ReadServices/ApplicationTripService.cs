using Application.QueryServices.ServiceInterfaces;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace Application.QueryServices;

public class ApplicationTripService : IApplicationTripService
{
    private readonly IQueryContext _readTrips;
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationTripServiceInMemory> _logger;

    public ApplicationTripService(IQueryContext readTrips, IDatetimeProvider datetimeProvider, ILogger<ApplicationTripServiceInMemory> logger)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    public async Task<ImmutableHashSet<Trip>> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        try
        {
            var materializedIds = GetUniqueStopIds(possibleSources, possibleDestinations);

            var tripIds = await GetRelevantTripProjectionAsync(materializedIds, _datetimeProvider.GetCurrentTime());

            var trips = await GetTripsByTripIdsAsync(tripIds);

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

    private async Task<List<string>> GetRelevantTripProjectionAsync(List<string> materializedIds, DateTime currentTime)
    {
        var tripIds = await _readTrips
            .GetData<ScheduledStop>()
            .Where(scheduledStop => materializedIds.Contains(scheduledStop.StopId) &&
                                    scheduledStop.DepartureTime > currentTime)
            .Select(scheduledStop => EF.Property<string>(scheduledStop, "TripId"))
            .Distinct()
            .ToListAsync();

        return tripIds;
    }

    private async Task<List<Trip>> GetTripsByTripIdsAsync(IEnumerable<string> tripIds)
    {
        return await _readTrips
            .GetData<Trip>()
            .Where(trip => tripIds.Contains(trip.Id))
            .Include(trip => trip.ScheduledStops)
            .ToListAsync();
    }
}