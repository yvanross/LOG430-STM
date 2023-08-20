using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Entities;
using Domain.Events.AggregateEvents.Trip;
using System.Collections.Immutable;

namespace Domain.Aggregates;

public class Trip : Aggregate<Trip>
{
    internal List<ScheduledStop> ScheduledStops { get; set;  }

    public Trip(string id, List<ScheduledStop> scheduledStops)
    {
        this.ScheduledStops = scheduledStops;
        Id = id;
    }

    internal static Trip CreateTrip(string tripId, IEnumerable<(string stopId, DateTime schedule)> stopSchedules)
    {
        return new Trip(tripId, stopSchedules.Select(x => new ScheduledStop(x.stopId, x.schedule)).ToList());
    }

    public override Trip Clone()
    {
        return new Trip(Id, ScheduledStops.Select(x => x.Clone()).ToList());
    }

    public bool IsTimeRelevant(IDatetimeProvider datetimeProvider)
    {
        if (ScheduledStops.Any() is false) throw new TripHasNoScheduledStopException();

        var lastStopTime = DeltaHours(ScheduledStops[^1].DepartureTime);

        return lastStopTime > 0;

        double DeltaHours(DateTime from)
        {
            var minutes = (from - datetimeProvider.GetCurrentTime()).TotalMinutes;

            return minutes;
        }
    }

    public bool ContainsStop(string stopId)
    {
        return ScheduledStops.Any(stopSchedule => stopSchedule.StopId.Equals(stopId));
    }

    public int NumberOfStops() => ScheduledStops.Count;

    public ScheduledStop GetStopByIndex(int index) => ScheduledStops[index].Clone();

    public int GetIndexOfStop(string stopId)
    {
        var index = ScheduledStops.FindIndex(stop => stop.StopId.Equals(stopId));

        if (index == -1) throw new ScheduledStopNotFoundException();

        return index;
    }

    public DateTime GetStopDepartureTime(string id)
    {
        return ScheduledStops.FirstOrDefault(stopSchedule => stopSchedule.Id.Equals(id))?.DepartureTime ??
               throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop FirstMatchingStop(Dictionary<string, Stop> stops)
    {
        var scheduledStop = ScheduledStops.FirstOrDefault(stopSchedule => stops.ContainsKey(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop LastMatchingStop(Dictionary<string, Stop> stops)
    {
        var scheduledStop = ScheduledStops.LastOrDefault(stopSchedule => stops.ContainsKey(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public void UpdateScheduledStops(string stopId, DateTime departureTime)
    {
        var stop = ScheduledStops.FirstOrDefault(scheduledStop => scheduledStop.StopId.Equals(stopId)) ?? throw new ScheduledStopNotFoundException();

        var updated = stop.UpdateDepartureTime(departureTime);

        if (updated)
            RaiseDomainEvent(new TripScheduledStopsUpdated(Id));
    }
}