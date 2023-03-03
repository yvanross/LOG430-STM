using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using StaticGTFS.Concretions;

namespace StaticGTFS;

public static class STMData
{
    public static ImmutableDictionary<string, ITrip>? Trips { get; private set; }
    public static IStop[]? Stops { get; private set; }

    public static void PrefetchData()
    {
        if (Stops is null && Trips is null)
        {
            var stops = FetchStopData().ToArray();

            Stops = stops;

            var stopImmutableDictionary = stops.ToImmutableDictionary(x => x.ID ?? Guid.NewGuid().ToString());

            var trips = FetchTripData().ToImmutableDictionary(x => x.ID ?? Guid.NewGuid().ToString());

            LinkTripAndStopData(stopImmutableDictionary, trips);

            Trips = trips;
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
                ID = info.GetValue("trip_id"),
            };
        }
    }

    private static void LinkTripAndStopData(ImmutableDictionary<string, IStop> stops, ImmutableDictionary<string, ITrip> trips)
    {
        var data = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES);

        for (int i = 0; i < data.Length; i++)
        {
            trips.TryGetValue(data[i].GetValue("trip_id") ?? string.Empty, out var trip);
            stops.TryGetValue(data[i].GetValue("stop_id") ?? string.Empty, out var tempStop);

            IStop stop = new Stop() {ID = tempStop.ID, Position = tempStop.Position};
            
            string? HMS = data[i].GetValue("arrival_time");

            DateTime departureTime = DateTime.UnixEpoch;
            
            if (HMS != null)
            {
                var HMSArray = HMS.Split(":");

                var dateTime = DateTime.UtcNow.Date;

                var hours = Convert.ToDouble(HMSArray[0]);

                dateTime = dateTime.AddHours(hours).AddMinutes(Convert.ToDouble(HMSArray[1])).AddSeconds(Convert.ToDouble(HMSArray[2]));

                if (DateTime.UtcNow.Hour < 12)
                {
                    dateTime = dateTime.Day > DateTime.UtcNow.Subtract(new TimeSpan(hours: 4, minutes: 0, seconds: 0)).Day ? dateTime.Subtract(new TimeSpan(hours: 24, minutes: 0, seconds: 0)) : dateTime;
                }

                //utc-4 to utc
                departureTime = dateTime.AddHours(4);
            }

            trip?.StopSchedules.Add(new StopSchedule()
            {
                Stop = stop,
                DepartureTime = departureTime
            });
        }
    }
}