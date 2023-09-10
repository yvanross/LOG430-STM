using System.Collections.Immutable;
using Domain.Common.Exceptions;
using Domain.Common.Seedwork.Abstract;

namespace Domain.Aggregates.Trip;

public class Trip : Aggregate<Trip>
{
    internal Trip(string id, List<ScheduledStop> scheduledStops) : base(id)
    {
        ScheduledStops = scheduledStops.ToImmutableList();
    }
    
    internal ImmutableList<ScheduledStop> ScheduledStops { get; set; }

    public string GetStopIdByIndex(int index)
    {
        var id = ScheduledStops[index].StopId;

        return id;
    }

    public int GetIndexOfStop(string stopId)
    {
        var index = ScheduledStops.FindIndex(stop => stop.StopId.Equals(stopId));

        if (index == -1) throw new ScheduledStopNotFoundException();

        return index;
    }

    public void ValidateStopIndex(int stopIndex)
    {
        if (stopIndex < 0 || stopIndex >= ScheduledStops.Count)
        {
            throw new IndexOutsideOfTripException("Stop index is out of range");
        }
    }
}