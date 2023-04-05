using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using Google.Protobuf.WellKnownTypes;
using GTFS.Concretions;
using GTFS.Interfaces;
using STM.ExternalServiceProvider.Proto;

namespace GTFS;

public class StmData : IgtfsDataSource
{
    private static ImmutableDictionary<string, IGTFSTrip>? _trips;

    private static ImmutableDictionary<string, IStopSTM>? _stops;

    public ImmutableDictionary<string, IGTFSTrip>? GetTrips() => _trips;

    public ImmutableDictionary<string, IStopSTM>? GetStops() => _stops;

    private static SemaphoreSlim semaphore = new (1);

    public void AddStop(IStopSTM igtfsStop)
    {
        if(_stops?.ContainsKey(igtfsStop.Id) is false)
            _stops = _stops.Add(igtfsStop.Id, igtfsStop);
    }

    public async Task PrefetchData()
    {
        await semaphore.WaitAsync();

        if (_stops is null && _trips is null)
        {
            var stops = FetchStopData().ToArray();

            _stops = stops.ToImmutableDictionary(x => x.Id);

            var trips = FetchTripData().ToImmutableDictionary(x => x.Id);

            var immutableTrips = LinkTripAndStopData(_stops, trips);

            _trips = immutableTrips;

            DynamicStaticGTFSParser.FlushData();

            semaphore.Release();
        }
    }

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates)
    {
        tripUpdates.ForEach(tu =>
        {
            var newOrUpdatedTrip = new GTFSTrip()
            {
                Id = tu.Trip.TripId,
                StopSchedules = tu.StopTimeUpdate.ToList().ConvertAll(stopTimeU =>
                {
                    if (_stops!.ContainsKey(stopTimeU.StopId) is false)
                        AddStop(new StopSTM() { Id = stopTimeU.StopId, Position = new PositionLL() });

                    var schedule = (IStopSchedule)new StopSchedule()
                    {
                        Stop = _stops[stopTimeU.StopId],
                        DepartureTime =
                            (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L)).AddHours(Timezone.UtcDiff)
                    };

                    return schedule;
                }),
            };

            if (_trips!.TryGetValue(newOrUpdatedTrip.Id, out var matchingTrip))
            {
                for (var i = 0; i < matchingTrip.StopSchedules.Count; i++)
                {
                    if (matchingTrip.StopSchedules[i].Stop.Id.Equals(newOrUpdatedTrip.Id))
                    {
                        matchingTrip.StopSchedules.RemoveRange(i, matchingTrip.StopSchedules.Count - i - 1);
                        matchingTrip.StopSchedules.AddRange(matchingTrip.StopSchedules);

                        break;
                    }
                }
            }
            else
                _trips = _trips!.Add(newOrUpdatedTrip.Id, newOrUpdatedTrip);
        });
    }

    private IEnumerable<IStopSTM> FetchStopData()
    {
        var stopsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.STOPS);

        foreach (var info in stopsInfo)
        {
            double.TryParse(info.GetValue("stop_lat"), out var lat);
            double.TryParse(info.GetValue("stop_lon"), out var lon);

            if (info.GetValue("stop_id") is { } id && string.IsNullOrEmpty(id) is false)
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

    private IEnumerable<IGTFSTrip> FetchTripData()
    {
        var tripsInfo = DynamicStaticGTFSParser.GetInfo(DataCategory.TRIPS);

        foreach (var info in tripsInfo)
        {
            if(info.GetValue("trip_id") is { } id && string.IsNullOrEmpty(id) is false)
                yield return new GTFSTrip() { Id = id };
        }
    }

    private ImmutableDictionary<string, IGTFSTrip> LinkTripAndStopData(ImmutableDictionary<string, IStopSTM> stops, ImmutableDictionary<string, IGTFSTrip> trips)
    {
        var datas = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES).ToList();

        foreach (var gtfsInfo in datas)
        {
            trips.TryGetValue(gtfsInfo.GetValue("trip_id") ?? string.Empty, out var trip);
            stops.TryGetValue(gtfsInfo.GetValue("stop_id") ?? string.Empty, out var tempStop);

            if (trip is null || tempStop is null) continue;

            var stop = new StopSTM() { Id = tempStop.Id, Position = tempStop.Position };

            string? HMS = gtfsInfo.GetValue("arrival_time");

            var departureTime = DateTime.UnixEpoch;

            if (HMS != null)
            {
                var HMSArray = HMS.Split(":");

                var dateTime = DateTime.UtcNow.Date;

                dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1]))
                    .AddSeconds(Convert.ToDouble(HMSArray[2])).AddHours(Timezone.UtcDiff);

                var dayModifier = (dateTime.Date.ToTimestamp().Seconds - DateTime.UtcNow.Date.ToTimestamp().Seconds) / TimeSpan.FromDays(1).TotalSeconds;

                dateTime = dateTime.AddDays(-(int)dayModifier);

                departureTime = dateTime;
            }

            trip.StopSchedules.Add(new StopSchedule()
            {
                Stop = stop,
                DepartureTime = departureTime
            });
        }

        return trips;
    }
}