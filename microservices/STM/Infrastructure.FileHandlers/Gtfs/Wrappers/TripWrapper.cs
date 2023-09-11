using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public sealed class TripWrapper : ITripWrapper
{
    private readonly GtfsFileFileCache _gtfsFileFileCache;
    private readonly GtfsInfo _info;
    private readonly WrapperMediator _mediator;
    private readonly IDatetimeProvider _datetimeProvider;

    public TripWrapper(
        GtfsInfo info,
        GtfsFileFileCache gtfsFileFileCache, 
        WrapperMediator mediator,
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

            var stopSequence = Convert.ToInt32(stopTime.GetValue("stop_sequence"));

            var hoursMinutesSeconds = stopTime.GetValue("arrival_time");

            var HMSArray = hoursMinutesSeconds.Split(":");

            var hours = int.Parse(HMSArray[0]);
            var minutes = int.Parse(HMSArray[1]);
            var seconds = int.Parse(HMSArray[2]);

            var timeSpan = new TimeSpan(hours, minutes, seconds);

            //since the gtfs in local time, we need to add the utc difference to get the correct UTC time
            timeSpan = timeSpan.Add(TimeSpan.FromHours(_datetimeProvider.GetUtcDifference()));

            var stopSchedule = new StopScheduleWrapper
            {
                DepartureTime = timeSpan,
                StopId = stopWrapper.Id,
                StopSequence = stopSequence
            };

            if (schedules.TryGetValue(tripId, out var stopSchedules))
                stopSchedules.Add(stopSchedule);
            else
                schedules.Add(tripId, new List<IStopScheduleWrapper> { stopSchedule });

            stopTime.Dispose();
        }

        return schedules;
    }
}