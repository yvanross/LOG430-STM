using Domain.Common.Seedwork.Abstract;
using Domain.Common.Seedwork.Interfaces;

namespace Domain.Entities;

public class ScheduledStop : Entity<ScheduledStop>
{
    public string StopId { get; private set; }
    
    public DateTime DepartureTime { get; private set; }

    public ScheduledStop(string stopId, DateTime departureTime)
    {
        StopId = stopId;
        DepartureTime = departureTime;
    }

    public bool Equals(ScheduledStop? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StopId == other.StopId && DepartureTime.Equals(other.DepartureTime);
    }

    public override ScheduledStop Clone()
    {
        return new ScheduledStop(StopId, DepartureTime);
    }
}