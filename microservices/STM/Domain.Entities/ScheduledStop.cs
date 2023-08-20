using Domain.Common.Seedwork.Abstract;

namespace Domain.Entities;

public class ScheduledStop : Entity<ScheduledStop>
{
    public string StopId { get; internal set; }

    public DateTime DepartureTime { get; internal set; }

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

    internal bool UpdateDepartureTime(DateTime departureTime)
    {
        if (DepartureTime.Equals(departureTime)) return false;

        DepartureTime = departureTime;

        return true;
    }
}