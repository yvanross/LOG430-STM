using Entities.Domain;

namespace STM.Entities.Concretions;

public struct StopScheduleSTM : IStopSchedule
{
    public IStop Stop { get; internal set; }

    public DateTime DepartureTime { get; internal set; }

    public int index { get; set; }

    public object Clone()
    {
        return this with { Stop = (IStop)Stop.Clone() };
    }
}