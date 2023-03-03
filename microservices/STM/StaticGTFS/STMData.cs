using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using GTFS.Concretions;
using GTFS.Interfaces;
using STM.ExternalServiceProvider.Proto;

namespace GTFS;

public static class STMData
{
    public static ImmutableList<IGTFSTrip>? Trips { get; private set; }
    public static ImmutableDictionary<string, IStopSTM>? Stops { get; private set; }

    public static void AddTrip(GTFSTrip gtfsTrip)
    {
        Trips = Trips?.Add(gtfsTrip);
    }

    public static void AddStop(IStopSTM igtfsStop)
    {
        if(Stops?.ContainsKey(igtfsStop.Id) is false)
            Stops = Stops.Add(igtfsStop.Id, igtfsStop);
    }

    public static void PrefetchData()
    {
        if (Stops is null && Trips is null)
        {
            var stops = FetchStopData().ToArray();

            Stops = stops.ToImmutableDictionary(x => x.Id);

            var trips = FetchTripData().ToImmutableDictionary(x => x.Id ?? Guid.NewGuid().ToString());

            var immutableTrips = LinkTripAndStopData(Stops, trips);

            Trips = immutableTrips;
        }
    }

    public static List<IGTFSTrip> AddRealtimeGTFSData(List<TripUpdate> realtimeGTFS)
    {
        var realtimeTrips = new List<IGTFSTrip>();

        realtimeGTFS.ForEach(x => realtimeTrips.Add(new GTFSTrip()
        {
            Id = x.Trip.TripId,
            StopSchedules = x.StopTimeUpdate.ToList().ConvertAll(stopTimeU =>
            {
                if (STMData.Stops!.ContainsKey(stopTimeU.StopId) is false)
                    STMData.AddStop(new StopSTM() { Id = stopTimeU.StopId, Position = new PositionLL() });

                var schedule = (IStopSchedule)new StopSchedule()
                {
                    Stop = STMData.Stops[stopTimeU.StopId],
                    DepartureTime = (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L))
                };

                return schedule;
            }),
            FromStaticGtfs = false
        }));

        return realtimeTrips;
    }

    private static IEnumerable<IStopSTM> FetchStopData()
    {
        var stopsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.STOPS);

        foreach (var info in stopsInfo)
        {
            double.TryParse(info.GetValue("stop_lat"), out var lat);
            double.TryParse(info.GetValue("stop_lon"), out var lon);

            yield return new StopSTM()
            {
                Id = info.GetValue("stop_id"),
                Position = new PositionLL()
                {
                    Latitude = lat,
                    Longitude = lon
                }
            };
        }
    }

    private static IEnumerable<IGTFSTrip> FetchTripData()
    {
        var tripsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.TRIPS);

        foreach (var info in tripsInfo)
        {
            yield return new GTFSTrip()
            {
                Id = info.GetValue("trip_id"),
            };
        }
    }

    private static ImmutableList<IGTFSTrip> LinkTripAndStopData(ImmutableDictionary<string, IStopSTM> stops, ImmutableDictionary<string, IGTFSTrip> trips)
    {
        var datas = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES).ToList();

        var doubledTrips = trips.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => new[] { keyValuePair.Value, (IGTFSTrip)keyValuePair.Value.Clone(), (IGTFSTrip)keyValuePair.Value.Clone() });

        Parallel.ForEach(new [] { 0,1,2}, (day) =>
        {
            foreach (var gtfsInfo in datas)
            {
                doubledTrips.TryGetValue(gtfsInfo.GetValue("trip_id") ?? string.Empty, out var trip);
                stops.TryGetValue(gtfsInfo.GetValue("stop_id") ?? string.Empty, out var tempStop);

                if (trip is null || tempStop is null) continue;

                var stop = new StopSTM() { Id = tempStop.Id, Position = tempStop.Position };

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