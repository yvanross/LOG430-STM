using System.Collections.Immutable;
using Domain.Common.Exceptions;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Trip;

namespace Domain.Aggregates.Trip;

public class Trip : Aggregate<Trip>
{
    private Trip(string id, List<ScheduledStop> scheduledStops)
    {
        ScheduledStops = scheduledStops.ToImmutableList();
        Id = id;
    }
    
    internal ImmutableList<ScheduledStop> ScheduledStops { get; set; }

    internal static Trip CreateTrip(string tripId, List<(string id, string stopId, TimeSpan departureTimeSpan)> stopSchedules)
    {
        var scheduledStops = stopSchedules
            .Select(x => new ScheduledStop(x.id, x.stopId, x.departureTimeSpan))
            .ToList();

        var trip = new Trip(tripId, scheduledStops);

        trip.RaiseDomainEvent(new TripCreated(tripId, scheduledStops.Select(st => st.Id).ToHashSet()));

        return trip;
    }

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