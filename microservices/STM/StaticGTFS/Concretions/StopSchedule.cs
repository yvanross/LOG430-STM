using Entities.Domain;

namespace StaticGTFS.Concretions;

public struct StopSchedule : IStopSchedule
{
    public IStop Stop { get; set; }

    public DateTime DepartureTime { get; set; }

    public object Clone()
    {
        return this with { Stop = (IStop)Stop.Clone() };
    }
}