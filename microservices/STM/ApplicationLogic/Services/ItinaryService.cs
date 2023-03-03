using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using GTFS;
using GTFS.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services;

public class ItinaryService
{
    private ILogger? _logger;

    public ItinaryService(ILogger? logger)
    {
        _logger = logger;
    }

    public IStopSTM[]? GetClosestStops(IPosition position, IStopSTM[]? stops, int RadiusForBusStopSelection = 50)
    {
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
        
        return closestStops.ToArray();

        List<IStopSTM> GetOtherNearbyStops(int radiusForBusStopSelection)
        {
            List<IStopSTM> otherStops = new List<IStopSTM>();

            for (int i = 0; i < stops?.Length; i++)
            {
                var temp = DistanceInMeters(closestStop.Position, stops[i].Position);

                var relativeMinDistance = minDistance * 1.1;

                if ((relativeMinDistance > temp && relativeMinDistance < (radiusForBusStopSelection * 2)) ||
                    radiusForBusStopSelection > temp)
                {
                    otherStops.Add(stops[i]);
                }
            }

            return otherStops.Count > 4 ? otherStops : GetOtherNearbyStops(radiusForBusStopSelection + 25);
        }
    }

    public ITripSTM[] TripsContainingSourceAndDestination(IStopSTM[] possibleSources, IStopSTM[] possibleDestinations, List<IGTFSTrip>? trips)
    {
        bool exit = false;

        double DeltaHours(DateTime from)
        {
            var hours = (from - DateTime.UtcNow).TotalHours;
            return hours;
        }

        bool FilterTripsTimeThatAreNotRelevant(IGTFSTrip t)
        {
            bool keep = true;

            if (!t.StopSchedules.Any()) return false;

            var firstStopTime = DeltaHours(t.StopSchedules[0].DepartureTime);
            var lastStopTime = DeltaHours(t.StopSchedules[^1].DepartureTime);

            if (firstStopTime is > 1 or < -3) keep = false;

            if (lastStopTime is > 4 or < 0) keep = false;

            return keep;
        }

        var relevantRealtimeTrips = trips?.Where(FilterTripsTimeThatAreNotRelevant).ToList();
        
        var relevantStaticTrips = STMData.Trips?.Where(FilterTripsTimeThatAreNotRelevant).ToList();

        var relevantTripArray = relevantStaticTrips.Concat(relevantRealtimeTrips).ToList();

        var possibleTrips = relevantTripArray?.AsParallel().Select((trip, _) =>
        {
            for (int i = 0; i < possibleSources.Length; i++)
            {
                for (int j = 0; j < possibleDestinations.Length; j++)
                {
                    if (trip.StopSchedules.Any(s => s.Stop.Id.Equals(possibleSources[i].Id)) &&
                        trip.StopSchedules.Any(s => s.Stop.Id.Equals(possibleDestinations[j].Id)))
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
                            RelevantOriginStopId = possibleSources[i].Id,
                            RelevantDestinationStopId = possibleDestinations[j].Id,
                            FromStaticGtfs = trip.FromStaticGtfs
                        };

                        return stmTrip;
                    }
                }
            }

            return null;
        }).OfType<ITripSTM>().ToArray();

        return possibleTrips!;
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

        return (h2 * earthRadius)*1000;
    }

    public double ToRad(double degree) => degree * (Math.PI / 180);

    public double Relative(double rad) => rad > 0 ? rad : (2 * Math.PI) - Math.Abs(rad);

}