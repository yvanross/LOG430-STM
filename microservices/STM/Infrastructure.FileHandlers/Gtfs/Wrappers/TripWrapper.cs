using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Google.Protobuf.WellKnownTypes;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public sealed class TripWrapper : ITripWrapper
{
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly GtfsFileFileCache _gtfsFileFileCache;

    private readonly GtfsInfo _info;
    private readonly WrapperMediator _mediator;

    public TripWrapper(GtfsInfo info, GtfsFileFileCache gtfsFileFileCache, WrapperMediator mediator,
        IDatetimeProvider datetimeProvider)
    {
        _info = info;
        _gtfsFileFileCache = gtfsFileFileCache;
        _mediator = mediator;
        _datetimeProvider = datetimeProvider;

        TripId = GetTripId();

        GlobalScheduledStops ??= GetStopWrappers();

        ScheduledStops = GlobalScheduledStops[TripId];

        GlobalScheduledStops.Remove(TripId);
    }

    private static Dictionary<string, List<IStopScheduleWrapper>>? GlobalScheduledStops { get; set; }
    public string TripId { get; }

    public List<IStopScheduleWrapper> ScheduledStops { get; }

    public void Dispose()
    {
        ScheduledStops.Clear();
    }

    private string GetTripId()
    {
        return _info.GetValue("trip_id");
    }

    private Dictionary<string, List<IStopScheduleWrapper>> GetStopWrappers()
    {
        var schedules = new Dictionary<string, List<IStopScheduleWrapper>>();

        foreach (var stopTime in _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOP_TIMES))
        {
            if (_mediator.Stops.TryGetValue(stopTime.GetValue("stop_id"), out var stopWrapper) is false)
                throw new NullReferenceException(
                    "Stop was null, this is a critical failure, make sure the data is accessible");

            if (stopTime.GetValue("trip_id") is not { } tripId || string.IsNullOrEmpty(tripId))
                throw new NullReferenceException(
                    "TripId was null, this is a critical failure, make sure the data is accessible");

            var hoursMinutesSeconds = stopTime.GetValue("arrival_time");

            var timeSpan = TimeSpan.Parse(hoursMinutesSeconds);

            var stopSchedule = new StopScheduleWrapper
            {
                DepartureTime = timeSpan,
                StopId = stopWrapper.Id
            };

            if (schedules.TryGetValue(tripId, out var stopSchedules))
                stopSchedules.Add(stopSchedule);
            else
                schedules.Add(tripId, new List<IStopScheduleWrapper> { stopSchedule });

            stopTime.Dispose();
        }

        return schedules;
    }

    //private Dictionary<string, List<IStopScheduleWrapper>> GetStopWrappers()
    //{
    //    var schedules = new Dictionary<string, List<IStopScheduleWrapper>>();

    //    foreach (var stopTime in _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOP_TIMES))
    //    {
    //        if (_mediator.Stops.TryGetValue(stopTime.GetValue("stop_id"), out var stopWrapper) is false)
    //            throw new NullReferenceException(
    //                "Stop was null, this is a critical failure, make sure the data is accessible");

    //        if (stopTime.GetValue("trip_id") is not { } tripId || string.IsNullOrEmpty(tripId))
    //            throw new NullReferenceException(
    //                "TripId was null, this is a critical failure, make sure the data is accessible");

    //        var HMS = stopTime.GetValue("arrival_time");

    //        var HMSArray = HMS.Split(":");

    //        var dateTime = DateTime.UtcNow.Date;

    //        dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1]))
    //            .AddSeconds(Convert.ToDouble(HMSArray[2])).AddHours(_datetimeProvider.GetUtcDifference());

    //        var differenceInDaysBetweenStopTimeAndCurrentTime =
    //            (dateTime.Date.ToTimestamp().Seconds - _datetimeProvider.GetCurrentTime().Date.ToTimestamp().Seconds) /
    //            TimeSpan.FromDays(1).TotalSeconds;

    //        dateTime = dateTime.AddDays(-(int)differenceInDaysBetweenStopTimeAndCurrentTime);

    //        var stopSchedule = new StopScheduleWrapper
    //        {
    //            DepartureTime = dateTime,
    //            StopId = stopWrapper.Id
    //        };

    //        if (schedules.TryGetValue(tripId, out var stopSchedules))
    //            stopSchedules.Add(stopSchedule);
    //        else
    //            schedules.Add(tripId, new List<IStopScheduleWrapper> { stopSchedule });

    //        stopTime.Dispose();
    //    }

    //    return schedules;
    //}
}