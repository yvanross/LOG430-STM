using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public struct StopScheduleWrapper : IStopScheduleWrapper
{
    public string StopId { get; set; }

    public DateTime DepartureTime { get; set; }
}