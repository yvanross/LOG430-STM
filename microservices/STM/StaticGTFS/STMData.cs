using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using StaticGTFS.Concretions;

namespace StaticGTFS;

public static class STMData
{
    public static ImmutableList<ITrip>? Trips { get; private set; }
    public static ImmutableDictionary<string, IStop>? Stops { get; private set; }

    public static void AddTrip(Trip trip)
    {
        Trips = Trips?.Add(trip);
    }

    public static void AddStop(Stop stop)
    {
        if(Stops?.ContainsKey(stop.ID) is false)
            Stops = Stops.Add(stop.ID, stop);
    }

    public static void PrefetchData()
    {
        if (Stops is null && Trips is null)
        {
            var stops = FetchStopData().ToArray();

            Stops = stops.ToImmutableDictionary(x => x.ID ?? Guid.NewGuid().ToString());

            var trips = FetchTripData().ToImmutableDictionary(x => x.Id ?? Guid.NewGuid().ToString());

            var immutableTrips = LinkTripAndStopData(Stops, trips);

            Trips = immutableTrips;
        }
    }

    private static IEnumerable<IStop> FetchStopData()
    {
        var stopsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.STOPS);

        foreach (var info in stopsInfo)
        {
            double.TryParse(info.GetValue("stop_lat"), out var lat);
            double.TryParse(info.GetValue("stop_lon"), out var lon);

            yield return new Stop()
            {
                ID = info.GetValue("stop_id"),
                Position = new Position()
                {
                    Latitude = lat,
                    Longitude = lon
                }
            };
        }
    }

    private static IEnumerable<ITrip> FetchTripData()
    {
        var tripsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.TRIPS);

        foreach (var info in tripsInfo)
        {
            yield return new Trip()
            {
                Id = info.GetValue("trip_id"),
            };
        }
    }

    private static ImmutableList<ITrip> LinkTripAndStopData(ImmutableDictionary<string, IStop> stops, ImmutableDictionary<string, ITrip> trips)
    {
        var datas = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES).ToList();

        var doubledTrips = trips.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => new[] { keyValuePair.Value, (ITrip)keyValuePair.Value.Clone(), (ITrip)keyValuePair.Value.Clone() });

        Parallel.ForEach(new [] { 0,1,2}, (day) =>
        {
            foreach (var gtfsInfo in datas)
            {
                doubledTrips.TryGetValue(gtfsInfo.GetValue("trip_id") ?? string.Empty, out var trip);
                stops.TryGetValue(gtfsInfo.GetValue("stop_id") ?? string.Empty, out var tempStop);

                if (trip is null || tempStop is null) continue;

                IStop stop = new Stop() { ID = tempStop.ID, Position = tempStop.Position };

                string? HMS = gtfsInfo.GetValue("arrival_time");

                DateTime departureTime = DateTime.UnixEpoch;

                if (HMS != null)
                {
                    var HMSArray = HMS.Split(":");

                    var dateTime = DateTime.UtcNow.Date;

                    dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1])).AddSeconds(Convert.ToDouble(HMSArray[2])).AddHours(5);

                    dateTime = dateTime.AddDays(day - 1);

                    departureTime = dateTime;
                }

                trip[day].StopSchedules.Add(new StopSchedule()
                {
                    Stop = stop,
                    DepartureTime = departureTime
                });
            }

        });

        return doubledTrips.SelectMany(d => d.Value).ToImmutableList();
    }
}