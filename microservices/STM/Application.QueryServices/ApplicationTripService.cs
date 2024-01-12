using Application.Dtos;
using Application.QueryServices.ServiceInterfaces;
using Domain.Aggregates.Stop;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryServices;

public class ApplicationTripService
{
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ILogger<ApplicationTripService> _logger;
    private readonly IQueryContext _readTrips;

    public ApplicationTripService(IQueryContext readTrips, IDatetimeProvider datetimeProvider,
        ILogger<ApplicationTripService> logger)
    {
        _readTrips = readTrips;
        _datetimeProvider = datetimeProvider;
        _logger = logger;
    }

    public async Task<Dictionary<string, List<ScheduledStopDto>>> TimeRelevantTripsContainingSourceAndDestination(
        IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations)
    {
        try
        {
            var materializedIds = GetUniqueStopIds(possibleSources, possibleDestinations);

            var tripProjection = await GetRelevantTripIdsAsync(materializedIds);

            return tripProjection;
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

    private async Task<Dictionary<string, List<ScheduledStopDto>>> GetRelevantTripIdsAsync(List<string> materializedIds)
    {
        //For simplicity's sake, I will assume UTC-4 as the timezone for montreal and not account for daylight savings time.
        //Out of the box the static GTFS comes in a 30-hour window.
        //Converting that to UTC and saving to the DB means pushing all timespans by 4 hours.
        //We are now on a weird stretch with a maximum of 34 hours and a minimum of 4 hours.
        //Issue being that a trip at 25 hours might wrap at hour 1, being the next day.
        //We can assume that no user selects two points more than 4 hours apart on a same trip (heuristic)

        var numberOfHoursInADayOnEarth = TimeSpan.FromHours(24);

        var maxAheadOfTimeAllowed = TimeSpan.FromHours(4);

        var currentTimeOfDay = _datetimeProvider.GetCurrentTime().TimeOfDay;

        var scheduledStops = await _readTrips
            .GetData<ScheduledStopDto>()
            .Where(scheduledStop => materializedIds.Contains(scheduledStop.StopId))
            .ToListAsync();

        var scheduledStopsGroupings = scheduledStops
            .Where(scheduledStop =>
            
                //if the departure time is greater than 24, it has meaning tomorrow and today (it wraps around)
                scheduledStop.DepartureTimespan >= numberOfHoursInADayOnEarth ?
                    //then we need to check if the the today portion is is within a reasonable range of the current time or
                    (scheduledStop.DepartureTimespan.TotalSeconds % numberOfHoursInADayOnEarth.TotalSeconds) - maxAheadOfTimeAllowed.TotalSeconds < currentTimeOfDay.TotalSeconds ||
                    //if the tomorrow portion is in a reasonable range of the current time (in the event that the current time is late in the day) 
                    scheduledStop.DepartureTimespan - maxAheadOfTimeAllowed < currentTimeOfDay :
                    //if the departure time is less than 24, it only has meaning today
                    scheduledStop.DepartureTimespan - maxAheadOfTimeAllowed < currentTimeOfDay
            )

            .GroupBy(scheduledStop => scheduledStop.TripId)
            .Where(g => g.Count() > 1)
            .ToList();

        Dictionary<string, List<ScheduledStopDto>> tripProjection = scheduledStopsGroupings.ToDictionary(
            group => group.Key,
            group => group.ToList()
        );

        return tripProjection;
    }
}