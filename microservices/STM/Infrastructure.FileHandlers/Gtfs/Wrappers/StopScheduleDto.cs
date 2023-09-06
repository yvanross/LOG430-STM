using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public struct StopScheduleWrapper : IStopScheduleWrapper
{
    public string StopId { get; set; }

    public TimeSpan DepartureTime { get; set; }

    public int StopSequence { get; set; }
}