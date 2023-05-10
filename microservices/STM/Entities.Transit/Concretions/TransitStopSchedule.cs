using Entities.Common.Interfaces;

namespace Entities.Transit.Concretions;

public struct TransitStopSchedule : IStopSchedule
{
    public IStop Stop { get; set; }

    public DateTime DepartureTime { get; set; }

    public int Index { get; set; }
}