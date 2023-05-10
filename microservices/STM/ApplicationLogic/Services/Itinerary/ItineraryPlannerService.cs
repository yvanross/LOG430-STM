using ApplicationLogic.Interfaces;
using Entities.Common.Concretions;
using Entities.Common.Interfaces;
using Entities.Gtfs.Interfaces;
using Entities.Transit.Concretions;
using Entities.Transit.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.Itinerary;

public class ItineraryPlannerService
{
    private ILogger _logger;

    private readonly ITransitDataCache _stmData;

    public ItineraryPlannerService(ILogger<ItineraryPlannerService> logger, ITransitDataCache stmData)
    {
        _logger = logger;
        _stmData = stmData;
    }

    public Dictionary<string, IStop> GetClosestStops(IPosition position, IStop[]? stops, int radiusForBusStopSelection = 50)
    {
        var closestStop = FindClosestStop(position, stops);

        var closestStops = closestStop != null && !closestStop.Id.Equals(string.Empty) ? 
            GetOtherNearbyStops(closestStop.Position, stops, radiusForBusStopSelection)
            : new List<IStop>();

        return closestStops.ToDictionary(cs => cs.Id);
    }

    private IStop? FindClosestStop(IPosition position, IStop[]? stops)
    {
        var minDistance = double.MaxValue;
        
        IStop? closestStop = null;

        foreach (var stop in stops ?? Enumerable.Empty<IStop>())
        {
            var tempDistance = DistanceInMeters(position, stop.Position);
            
            if (minDistance > tempDistance)
            {
                minDistance = tempDistance;
                closestStop = stop;
            }
        }

        return closestStop;
    }

    private List<IStop> GetOtherNearbyStops(IPosition referencePosition, IStop[]? stops, int radiusForBusStopSelection)
    {
        var nearbyStops = (
            from stop in stops
            let temp = DistanceInMeters(referencePosition, stop.Position)
            where radiusForBusStopSelection > temp 
            select stop)
            .ToList();

        return nearbyStops.Count > 15 ?
                nearbyStops :
                GetOtherNearbyStops(referencePosition, stops, radiusForBusStopSelection + 50);
    }

    public Dictionary<string, ITransitTrip> TripsContainingSourceAndDestination(Dictionary<string, IStop> possibleSources, Dictionary<string, IStop> possibleDestinations)
    {
        var trips = _stmData.GetTrips();

        var relevantTrips = trips?.Values.Where(FilterTripsTimeThatAreNotRelevant).ToArray();

        var possibleTrips = relevantTrips?.AsParallel().AsUnordered().Select(CreateRelevantTrip).OfType<ITransitTrip>().ToDictionary(x => x.Id);

        return possibleTrips!;

        ITransitTrip? CreateRelevantTrip(ITrip trip, int _)
        {
            if (trip.StopSchedules.FirstOrDefault(s => possibleSources.ContainsKey(s.Stop.Id)) is { } sourceStop &&
                trip.StopSchedules.FirstOrDefault(s => possibleDestinations.ContainsKey(s.Stop.Id)) is { } destinationStop)
            {
                return new TransitTrip()
                {
                    Id = trip.Id,
                    StopSchedules = trip.StopSchedules.ConvertAll(stopSchedule => new TransitStopSchedule()
                    {
                        DepartureTime = stopSchedule.DepartureTime,
                        Stop = new Stop()
                        {
                            Id = stopSchedule.Stop.Id,
                            Position = stopSchedule.Stop.Position,
                        }
                    }),
                    RelevantOriginStopId = sourceStop.Stop.Id,
                    RelevantDestinationStopId = destinationStop.Stop.Id,
                };
            }

            return null;
        }

        bool FilterTripsTimeThatAreNotRelevant(ITrip t)
        {
            if (!t.StopSchedules.Any()) return false;

            var lastStopTime = DeltaHours(t.StopSchedules[^1].DepartureTime);

            return lastStopTime > 0;
        }

        double DeltaHours(DateTime from)
        {
            var minutes = (from - DateTime.UtcNow).TotalMinutes;

            return minutes;
        }
    }

    private double DistanceInMeters(IPosition a, IPosition b)
    {
        const int earthRadius = 6371;

        PositionLL aPrime = new PositionLL()
        {
            Latitude = ToRad(a.Latitude),
            Longitude = ToRad(a.Longitude)
        };

        PositionLL bPrime = new PositionLL()
        {
            Latitude = ToRad(b.Latitude),
            Longitude = ToRad(b.Longitude)
        };

        var lat = Math.Sin((bPrime.Latitude - aPrime.Latitude) / 2);
        var lon = Math.Sin((bPrime.Longitude - aPrime.Longitude) / 2);

        var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                 Math.Cos(aPrime.Latitude) * Math.Cos(bPrime.Latitude) *
                 Math.Sin(lon / 2) * Math.Sin(lon / 2);
        var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

        return h2 * earthRadius * 1000;
    }

    private static double ToRad(double degree) => degree * (Math.PI / 180);
}