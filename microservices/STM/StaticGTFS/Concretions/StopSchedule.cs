using Entities.Domain;

namespace GTFS.Concretions;

public struct StopSchedule : IStopSchedule
{
    public IStopSTM Stop { get; set; }

    public DateTime DepartureTime { get; set; }

    public object Clone()
    {
        return this with { Stop = (IStopSTM)Stop.Clone() };
    }
}