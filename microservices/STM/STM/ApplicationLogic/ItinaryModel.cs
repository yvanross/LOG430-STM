using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using STM.Entities.Concretions;
using STM.Entities.Domain;

namespace STM.ApplicationLogic;

public class ItinaryModel
{
    private ILogger? _logger;

    public ItinaryModel(ILogger? logger)
    {
        _logger = logger;
    }

    public IStop[]? GetClosestStops(IPosition position, IStop[]? stops, int RadiusForBusStopSelection = 50)
    {
        double minDistance = 1000;
        
        IStop? closestStop = null;

        List<IStop> closestStops = new List<IStop>();

        for (int i = 0; i < stops?.Length; i++)
        {
            var tempDistance = DistanceInMeters(position, stops[i].Position);
            
            if (minDistance > tempDistance)
            {
                minDistance = tempDistance;
                closestStop = stops[i];
            }
        }

        if (closestStop != null && !closestStop.ID.Equals(string.Empty))
        {
            closestStops.AddRange(GetOtherNearbyStops(RadiusForBusStopSelection));
        }
        
        return closestStops.ToArray();

        List<IStop> GetOtherNearbyStops(int radiusForBusStopSelection)
        {
            List<IStop> otherStops = new List<IStop>();

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

    public ITripSTM[]? TripsContainingSourceAndDestination(IStop[] possibleSources, IStop[] possibleDestinations, ImmutableList<ITrip>? trips)
    {
        var possibleTrips = new Dictionary<string, ITripSTM>();

        bool exit = false;

        double DeltaHours(DateTime from)
        {
            var hours = (from - DateTime.UtcNow).TotalHours;
            return hours;
        }

        var relevantTrips = trips?.Where(t =>
        {

            bool keep = true;

            if (!t.StopSchedules.Any()) return false;

            var firstStopTime = DeltaHours(t.StopSchedules[0].DepartureTime);
            var lastStopTime = DeltaHours(t.StopSchedules[^1].DepartureTime);

            if (firstStopTime is > 1 or < -3)
                keep = false;

            if (lastStopTime is > 4 or < 0)
                keep = false;

            return keep;
        });

        var relevantTripArray =  relevantTrips?.ToArray();

        for (int k = 0; k < relevantTripArray.Length; k++)
        {
            for (int i = 0; i < possibleSources.Length; i++)
            {
                for (int j = 0; j < possibleDestinations.Length; j++)
                {
                    if (relevantTripArray[k].StopSchedules.Any(s => s.Stop.ID.Equals(possibleSources[i].ID)) &&
                        relevantTripArray[k].StopSchedules.Any(s => s.Stop.ID.Equals(possibleDestinations[j].ID)) &&
                        !possibleTrips.ContainsKey(relevantTripArray[k].Id))
                    {
                        var stmTrip = new TripSTM()
                        {
                            ID = relevantTripArray[k].Id,
                            StopSchedules = relevantTripArray[k].StopSchedules.ConvertAll(stopSchedule =>
                                new StopScheduleSTM()
                                {
                                    DepartureTime = stopSchedule.DepartureTime,
                                    Stop = new StopSTM()
                                    {
                                        ID = stopSchedule.Stop.ID,
                                        Position = stopSchedule.Stop.Position,
                                    }
                                }),
                            RelevantOriginStopId = possibleSources[i].ID,
                            RelevantDestinationStopId = possibleDestinations[j].ID,
                            FromStaticGtfs = relevantTripArray[k].FromStaticGtfs
                        };

                        possibleTrips.Add(relevantTripArray[k].Id, stmTrip);

                        exit = true;
                    }

                    if (exit) break;
                }

                if (exit)
                {
                    exit = false;
                    break;
                }
            }
        }

        return possibleTrips.Values.ToArray();
    }

    private double DistanceInMeters(IPosition a, IPosition b)
    {
        const int earthRadius = 6371;

        Position aPrime = new Position()
        {
            Latitude = ToRad(a.Latitude),
            Longitude = ToRad(a.Longitude)
        };

        Position bPrime = new Position()
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