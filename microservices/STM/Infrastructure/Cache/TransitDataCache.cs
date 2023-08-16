using System.Collections.Concurrent;
using ApplicationLogic.Enums;
using ApplicationLogic.Helpers;
using ApplicationLogic.Interfaces;
using Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;

namespace Infrastructure2.Cache;

public class TransitDataCache : ITransitDataCache
{
    private readonly ILogger<TransitDataCache> _logger;
    private readonly GtfsFileFileCache _gtfsFileFileCache;

    private static ConcurrentDictionary<string, ITrip>? _trips;

    private static ConcurrentDictionary<string, IStop>? _stops;

    public ConcurrentDictionary<string, ITrip>? GetTrips() => _trips is null ? null : new(_trips);

    public ConcurrentDictionary<string, IStop>? GetStops() => _stops is null ? null : new(_stops);

    private static readonly SemaphoreSlim Semaphore = new (1);

    public TransitDataCache(ILogger<TransitDataCache> logger, GtfsFileFileCache gtfsFileFileCache)
    {
        _logger = logger;
        _gtfsFileFileCache = gtfsFileFileCache;
    }

    public void AddStop(IStop igtfsStop)
    {
        if(_stops?.ContainsKey(igtfsStop.Id) is false)
            _stops.AddOrUpdate(igtfsStop.Id, (_) => igtfsStop, (_, _) => igtfsStop);
    }

    public async Task PrefetchData()
    {
        _logger.LogInformation("# Attempting to prefetch cache...");

        if (_stops is null && _trips is null)
            await Semaphore.WaitAsync();

        if (_stops is null && _trips is null)
        {
            var stops = FetchStopData().ToDictionary(x => x.Id);

            var trips = FetchTripData().ToDictionary(x => x.Id);

            var linkedTrips = LinkTripAndStopData(stops, trips);

            _stops = new ConcurrentDictionary<string, IStop>(stops);
            _trips = new ConcurrentDictionary<string, ITrip>(linkedTrips);

            _logger.LogInformation("# Flushing file data...");

            _gtfsFileFileCache.FlushData();

            Semaphore.Release(int.MaxValue);

            _logger.LogInformation("# Ready");
        }
    }

    public void ApplyTripUpdatesToDataSet(List<TripUpdate> tripUpdates)
    {
        tripUpdates.ForEach(tu =>
        {
            var newOrUpdatedTrip = new Trip()
            {
                Id = tu.Trip.TripId,
                StopSchedules = tu.StopTimeUpdate.ToList().ConvertAll(stopTimeU =>
                {
                    if (_stops!.ContainsKey(stopTimeU.StopId) is false)
                        AddStop(new Stop() { Id = stopTimeU.StopId, Position = new PositionLL() });

                    var schedule = (IStopSchedule) new IndexedStopSchedule()
                    {
                        StopId = _stops[stopTimeU.StopId],
                        DepartureTime =
                            (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L)).AddHours(TimezoneHelper.UtcDiff)
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
                _trips!.AddOrUpdate(newOrUpdatedTrip.Id, (_) => newOrUpdatedTrip, (_,_) => newOrUpdatedTrip);
        });
    }

    private IEnumerable<IStop> FetchStopData()
    {
        var stopsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOPS);

        foreach (var info in stopsInfo)
        {
            double.TryParse(info.GetValue("stop_lat"), out var lat);
            double.TryParse(info.GetValue("stop_lon"), out var lon);

            if (info.GetValue("stop_id") is { } id && string.IsNullOrEmpty(id) is false)
                yield return new Stop()
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

    private IEnumerable<ITrip> FetchTripData()
    {
        var tripsInfo = _gtfsFileFileCache.GetInfo(DataCategoryEnum.TRIPS);

        foreach (var info in tripsInfo)
        {
            if(info.GetValue("trip_id") is { } id && string.IsNullOrEmpty(id) is false)
                yield return new Trip() { Id = id };
        }
    }

    private Dictionary<string, ITrip> LinkTripAndStopData(Dictionary<string, IStop> stops, Dictionary<string, ITrip> trips)
    {
        var datas = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOP_TIMES);

        if (datas is null) new NullReferenceException("Stop Times were null, this is a critical failure, make sure the data is accessible");
            
        foreach (var gtfsInfo in datas)
        {
            trips.TryGetValue(gtfsInfo.GetValue("trip_id") ?? string.Empty, out var trip);
            stops.TryGetValue(gtfsInfo.GetValue("stop_id") ?? string.Empty, out var tempStop);

            if (trip is null || tempStop is null) continue;

            var stop = new Stop() { Id = tempStop.Id, Position = tempStop.Position };

            string? HMS = gtfsInfo.GetValue("arrival_time");

            var departureTime = DateTime.UnixEpoch;

            if (HMS != null)
            {
                var HMSArray = HMS.Split(":");

                var dateTime = DateTime.UtcNow.Date;

                dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1]))
                    .AddSeconds(Convert.ToDouble(HMSArray[2])).AddHours(TimezoneHelper.UtcDiff);

                var dayModifier = (dateTime.Date.ToTimestamp().Seconds - DateTime.UtcNow.Date.ToTimestamp().Seconds) / TimeSpan.FromDays(1).TotalSeconds;

                dateTime = dateTime.AddDays(-(int)dayModifier);

                departureTime = dateTime;
            }

            trip.StopSchedules.Add(new ScheduledStop()
            {
                Stop = stop,
                DepartureTime = departureTime
            });
        }

        return trips;
    }
}