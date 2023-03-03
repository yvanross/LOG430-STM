using Entities.Domain;

namespace STM.Entities.Concretions;

public struct StopScheduleSTM : IStopSchedule
{
    public IStop Stop { get; internal set; }

    public DateTime DepartureTime { get; internal set; }

    public int index { get; set; }
}