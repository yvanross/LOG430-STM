using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Entities.Concretions;
using Entities.Domain;
using GTFS;
using GTFS.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services;

public class ItinaryService
{
    private ILogger? _logger;

    private readonly IgtfsDataSource _stmData;

    public ItinaryService(ILogger? logger, IgtfsDataSource stmData)
    {
        _logger = logger;
        _stmData = stmData;
    }

    public IStopSTM[]? GetClosestStops(IPosition position, IStopSTM[]? stops, int RadiusForBusStopSelection = 50)
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
        
        return closestStops.ToArray();

        List<IStopSTM> GetOtherNearbyStops(int radiusForBusStopSelection)
        {
            List<IStopSTM> otherStops = new List<IStopSTM>();

            for (int i = 0; i < stops?.Length; i++)
            {
                var temp = DistanceInMeters(closestStop.Position, stops[i].Position);

                var relativeMinDistance = minDistance * 2;

                if ((relativeMinDistance > temp && relativeMinDistance < (radiusForBusStopSelection * 2)) ||
                    radiusForBusStopSelection > temp)
                {
                    otherStops.Add(stops[i]);
                }
            }

            recursions++;

            return recursions > 100 ||
                   otherStops.Count > 15 ?
                    otherStops :
                    GetOtherNearbyStops(radiusForBusStopSelection + 50);
        }
    }

    public ImmutableDictionary<string, ITripSTM> TripsContainingSourceAndDestination(IStopSTM[] possibleSources, IStopSTM[] possibleDestinations)
    {
        var relevantTripArray = _stmData.GetTrips()?.Values.Where(FilterTripsTimeThatAreNotRelevant).ToArray();

        var possibleTrips = relevantTripArray?.AsParallel().AsUnordered().Select((trip, _) =>
        {
            ITripSTM? toReturn = null;

            Parallel.For(0, possibleSources.Length, (i, _) =>
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
                        };

                        toReturn = stmTrip;
                        return;
                    }
                }

            });

            return toReturn;
        }).OfType<ITripSTM>().ToImmutableDictionary(x=>x.Id);

        return possibleTrips!;

        bool FilterTripsTimeThatAreNotRelevant(IGTFSTrip t)
        {
            bool keep = false;

            if (!t.StopSchedules.Any()) return false;

            var firstStopTime = DeltaHours(t.StopSchedules[0].DepartureTime);
            var lastStopTime = DeltaHours(t.StopSchedules[^1].DepartureTime);

            if(lastStopTime > 0) 
                keep = true;

            return keep;
        }

        double DeltaHours(DateTime from)
        {
            var hours = (from - DateTime.UtcNow).TotalHours;
            return hours;
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

        return (h2 * earthRadius)*1000;
    }

    public double ToRad(double degree) => degree * (Math.PI / 180);
}