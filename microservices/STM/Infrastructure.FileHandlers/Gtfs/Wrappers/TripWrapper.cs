using Application.Mapping.Interfaces.Wrappers;
using Domain.Common.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Immutable;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public sealed class TripWrapper : ITripWrapper, IDisposable
{
    public string TripId { get; }

    public Lazy<ImmutableList<IStopScheduleWrapper>> ScheduledStops { get; private set; }

    private readonly GtfsInfo _info;
    private readonly GtfsFileFileCache _gtfsFileFileCache;
    private readonly WrapperMediator _mediator;
    private readonly IDatetimeProvider _datetimeProvider;

    public TripWrapper(GtfsInfo info, GtfsFileFileCache gtfsFileFileCache, WrapperMediator mediator, IDatetimeProvider datetimeProvider)
    {
        _info = info;
        _gtfsFileFileCache = gtfsFileFileCache;
        _mediator = mediator;
        _datetimeProvider = datetimeProvider;

        TripId = GetTripId();
        ScheduledStops = new(() => GetStopWrappers().ToImmutableList());
    }

    private string GetTripId()
    {
        return _info.GetValue("trip_id");
    }

    private IEnumerable<IStopScheduleWrapper> GetStopWrappers()
    {
        var stopTimes = _gtfsFileFileCache.GetInfo(DataCategoryEnum.STOP_TIMES);

        if (stopTimes is null)
            new NullReferenceException(
                "Stop Times were null, this is a critical failure, make sure the data is accessible");

        foreach (var stopTime in stopTimes)
        {
            if (_mediator.Stops.TryGetValue(stopTime.GetValue("stop_id"), out var stopWrapper) is false)
                throw new NullReferenceException("Stop was null, this is a critical failure, make sure the data is accessible");

            var HMS = stopTime.GetValue("arrival_time");

            var departureTime = DateTime.UnixEpoch;

            var HMSArray = HMS.Split(":");

            var dateTime = DateTime.UtcNow.Date;

            dateTime = dateTime.AddHours(Convert.ToDouble(HMSArray[0])).AddMinutes(Convert.ToDouble(HMSArray[1]))
                .AddSeconds(Convert.ToDouble(HMSArray[2])).AddHours(_datetimeProvider.GetUtcDifference());

            var dayModifier = (dateTime.Date.ToTimestamp().Seconds - _datetimeProvider.GetCurrentTime().Date.ToTimestamp().Seconds) / TimeSpan.FromDays(1).TotalSeconds;

            dateTime = dateTime.AddDays(-(int)dayModifier);

            departureTime = dateTime;

            yield return new StopScheduleWrapper(departureTime, stopWrapper.Id);
        }
    }

    public void Dispose()
    {
        _gtfsFileFileCache.Dispose();

        ScheduledStops = new(() => ImmutableList<IStopScheduleWrapper>.Empty);
    }
}