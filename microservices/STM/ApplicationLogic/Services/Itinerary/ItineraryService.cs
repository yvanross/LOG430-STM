using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using GTFS.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.Itinerary;

public class ItineraryService
{
    private ILogger? _logger;

    private readonly IgtfsDataSource _stmData;

    public ItineraryService(ILogger? logger, IgtfsDataSource stmData)
    {
        _logger = logger;
        _stmData = stmData;
    }

    public Dictionary<string, IStopSTM> GetClosestStops(IPosition position, IStopSTM[]? stops, int RadiusForBusStopSelection = 50)
    {
        int recursions = 0;

        double minDistance = 1000;

        IStopSTM? closestStop = null;

        List<IStopSTM> closestStops = new List<IStopSTM>();

        for (int i = 0; i < stops?.Length; i++)
        {
            var tempDistance = DistanceInMeters(position, stops[i].Position);

            if (minDistance > tempDistance)
            {
                minDistance = tempDistance;
                closestStop = stops[i];
            }
        }

        if (closestStop != null && !closestStop.Id.Equals(string.Empty))
        {
            closestStops.AddRange(GetOtherNearbyStops(RadiusForBusStopSelection));
        }

        return closestStops.ToDictionary(cs => cs.Id);

        List<IStopSTM> GetOtherNearbyStops(int radiusForBusStopSelection)
        {
            List<IStopSTM> otherStops = new List<IStopSTM>();

            for (int i = 0; i < stops?.Length; i++)
            {
                var temp = DistanceInMeters(closestStop.Position, stops[i].Position);

                var relativeMinDistance = minDistance * 2;

                if (relativeMinDistance > temp && relativeMinDistance < radiusForBusStopSelection * 2 ||
                    radiusForBusStopSelection > temp)
                {
                    otherStops.Add(stops[i]);
                }
            }

            recursions++;

            return recursions > 100 ||
                   otherStops.Count > 15 ?
                    otherStops :
                    GetOtherNearbyStops(radiusForBusStopSelection + 10);
        }
    }

    public ImmutableDictionary<string, ITripSTM> TripsContainingSourceAndDestination(Dictionary<string, IStopSTM> possibleSources, Dictionary<string, IStopSTM> possibleDestinations)
    {
        var trips = _stmData.GetTrips();

        var relevantTripArray = trips?.Values.Where(FilterTripsTimeThatAreNotRelevant).ToArray();

        var possibleTrips = relevantTripArray?.AsParallel().AsUnordered().Select((trip, _) =>
        {
            ITripSTM? toReturn = null;

            if (trip.StopSchedules.FirstOrDefault(s => possibleSources.ContainsKey(s.Stop.Id)) is { } sourceStop &&
                trip.StopSchedules.FirstOrDefault(s => possibleDestinations.ContainsKey(s.Stop.Id)) is { } destinationStop)
            {
                var stmTrip = new TripSTM()
                {
                    Id = trip.Id,
                    StopSchedules = trip.StopSchedules.ConvertAll(stopSchedule =>
                        new StopScheduleSTM()
                        {
                            DepartureTime = stopSchedule.DepartureTime,
                            Stop = new StopSTM()
                            {
                                Id = stopSchedule.Stop.Id,
                                Position = stopSchedule.Stop.Position,
                            }
                        }),
                    RelevantOriginStopId = sourceStop.Stop.Id,
                    RelevantDestinationStopId = destinationStop.Stop.Id,
                };

                toReturn = stmTrip;
            }

            return toReturn;
        }).OfType<ITripSTM>().ToImmutableDictionary(x => x.Id);

        return possibleTrips!;

        bool FilterTripsTimeThatAreNotRelevant(IGTFSTrip t)
        {
            var keep = false;

            if (!t.StopSchedules.Any()) return false;

            var lastStopTime = DeltaHours(t.StopSchedules[^1].DepartureTime);

            if (lastStopTime > 0)
                keep = true;

            return keep;
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

    public double ToRad(double degree) => degree * (Math.PI / 180);
}