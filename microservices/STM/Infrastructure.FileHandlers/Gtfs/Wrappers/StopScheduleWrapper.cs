using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public sealed class StopScheduleWrapper : IStopScheduleWrapper
{
    public string StopId { get; }

    public DateTime DepartureTime { get; }

    public StopScheduleWrapper(DateTime departureTime, string stopId)
    {
        DepartureTime = departureTime;
        StopId = stopId;
    }
}