using Entities.Domain;

namespace StaticGTFS.Concretions;

public struct StopSchedule : IStopSchedule
{
    public IStop Stop { get; internal set; }
    public DateTime DepartureTime { get; internal set; }
}