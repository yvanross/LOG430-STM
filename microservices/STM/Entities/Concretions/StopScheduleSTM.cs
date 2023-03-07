using Entities.Domain;

namespace Entities.Concretions;

public struct StopScheduleSTM : IStopSchedule
{
    public IStopSTM Stop { get; set; }

    public DateTime DepartureTime { get; set; }

    public int Index { get; set; }
}