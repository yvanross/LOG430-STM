using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Entities.Concretions;
using Entities.Domain;
using GTFS;
using GTFS.Concretions;
using GTFS.Interfaces;
using STM.ExternalServiceProvider.Proto;

namespace STMTests.Stub;

public class StmDataStub : IgtfsDataSource
{
    private static ImmutableDictionary<string, IGTFSTrip>? _trips;

    private static ImmutableDictionary<string, IStopSTM>? _stops;

    public ImmutableDictionary<string, IGTFSTrip>? GetTrips()
    {
        return _trips;
    }

    public ImmutableDictionary<string, IStopSTM>? GetStops()
    {
        return _stops;
    }

    public void AddStop(IStopSTM igtfsStop)
    {
        if (_stops?.ContainsKey(igtfsStop.Id) is false)
            _stops = _stops.Add(igtfsStop.Id, igtfsStop);
    }

    public async Task PrefetchData()
    {
        if (_trips is null && _stops is null)
        {
            var stops = FetchStopData().ToArray();

            _stops = stops.ToImmutableDictionary(x => x.Id);

            var trips = FetchTripData().ToImmutableDictionary(x => x.Id ?? Guid.NewGuid().ToString());

            var immutableTrips = LinkTripAndStopData(_stops, trips);

            _trips = immutableTrips;
        }
    }

    private ImmutableDictionary<string, IGTFSTrip>? LinkTripAndStopData(ImmutableDictionary<string, IStopSTM> stops, ImmutableDictionary<string, IGTFSTrip> trips)
    {
        var datas = new List<GTFSInfo>();

        var random = new Random();

        for (var i = 0; i < 10000; i++)
        {
            var date = DateTime.Now.AddHours(random.Next(-12, 12));
            var time = $"{date.Hour}:{date.Minute}:{date.Second}";

            datas.Add(new GTFSInfo()
            {
                Info = new Dictionary<string, string>()
                {
                    {"trip_id", i.ToString()},
                    {"stop_id", i.ToString()},
                    {"arrival_time", time} 
                }
            });
        }

        Parallel.ForEach(new[] { 0, 1, 2 }, (day) =>
        {
            foreach (var gtfsInfo in datas)
            {
                trips.TryGetValue(gtfsInfo.GetValue("trip_id") ?? string.Empty, out var trip);
                stops.TryGetValue(gtfsInfo.GetValue("stop_id") ?? string.Empty, out var tempStop);

                if (trip is null || tempStop is null) continue;

                var stop = new StopSTM() { Id = tempStop.Id, Position = tempStop.Position };

                string? HMS = gtfsInfo.GetValue("arrival_time");

                DateTime departureTime = DateTime.UnixEpoch;

                if (HMS != null)
                {
                    var HMSArray = HMS.Split(":");

                    var dateTime = DateTime.UtcNow.Date;

                    dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1])).AddSeconds(Convert.ToDouble(HMSArray[2]));

                    departureTime = dateTime;
                }

                trip.StopSchedules.Add(new StopSchedule()
                {
                    Stop = stop,
                    DepartureTime = departureTime
                });
            }
        });

        return trips;
    }

    private List<IGTFSTrip> FetchTripData()
    {
        var trips = new List<IGTFSTrip>();

        for (var i = 0; i < 10000; i++)
        {
            trips.Add(new GTFSTrip()
            {
                Id = i.ToString()
            });
        }

        return trips;
    }

    private List<IStopSTM> FetchStopData()
    {
        var trips = new List<IStopSTM>();

        for (var i = 0; i < 10000; i++)
        {
            trips.Add(new StopSTM()
            {
                Id = i.ToString(),
                Message = "this is a stub",
                Position = new PositionLL()
                {
                    Longitude = i,
                    Latitude = i,
                }
            });
        }

        return trips;
    }

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates)
    {
        /*
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
                            (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L))
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
        */
    }
}