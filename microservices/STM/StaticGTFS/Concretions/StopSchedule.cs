using Entities.Domain;

namespace GTFS.Concretions;

public struct StopSchedule : IStopSchedule
{
    public IStopSTM Stop { get; set; }

    public DateTime DepartureTime { get; set; }
}