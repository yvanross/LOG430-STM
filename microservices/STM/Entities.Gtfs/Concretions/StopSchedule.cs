using Entities.Common.Interfaces;

namespace Entities.Gtfs.Concretions;

public struct StopSchedule : IStopSchedule
{
    public IStop Stop { get; set; }

    public DateTime DepartureTime { get; set; }
}