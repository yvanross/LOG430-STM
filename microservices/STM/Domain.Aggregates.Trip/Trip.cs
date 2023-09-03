using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Trip;

namespace Domain.Aggregates.Trip;

public class Trip : Aggregate<Trip>
{
    //Risk of race condition but since dbcontext is scoped (one instance per request) it should be fine. Unless there's an attempt to multi thread updates to the same trip (why?).
    //Considering the headaches it saves from EF Core, it's worth it.
    internal List<ScheduledStop> ScheduledStops { get; set; }

    private Trip(){}

    private Trip(string id, List<ScheduledStop> scheduledStops)
    {
        ScheduledStops = scheduledStops;
        Id = id;
    }

    internal static Trip CreateTrip(string tripId, IEnumerable<(string stopId, DateTime schedule)> stopSchedules)
    {
        var scheduledStops = stopSchedules
            .Select(x => new ScheduledStop(Guid.NewGuid().ToString(), x.stopId, x.schedule))
            .ToList();

        var trip = new Trip(tripId, scheduledStops);

        trip.RaiseDomainEvent(new TripCreated(tripId, scheduledStops.Select(st => st.Id).ToHashSet()));

        return trip;
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

    public bool IsDepartureAndDestinationInRightOrder(string departureStopId, string destinationStopId)
    {
        return GetIndexOfStop(departureStopId) < GetIndexOfStop(destinationStopId);
    }

    public bool ContainsStop(string stopId)
    {

        var condition = ScheduledStops.Any(stopSchedule => stopSchedule.StopId.Equals(stopId));

        return condition;
    }

    public int NumberOfStops()
    {
        var count = ScheduledStops.Count;

        return count;
    }

    public string GetStopIdByIndex(int index)
    {
        var id = ScheduledStops[index].StopId;

        return id;
    }

    public string GetScheduleIdByStopId(string stopId)
    {
        var id = ScheduledStops.FirstOrDefault(scheduledStop => scheduledStop.StopId.Equals(stopId))?.Id ?? 
                 throw new ScheduledStopNotFoundException();

        return id;
    }

    public int GetIndexOfStop(string stopId)
    {
        var index = ScheduledStops.FindIndex(stop => stop.StopId.Equals(stopId));

        if (index == -1) throw new ScheduledStopNotFoundException();

        return index;
    }

    public ScheduledStop FirstMatchingStop(HashSet<string> stopdIds)
    {
        var scheduledStop = ScheduledStops.FirstOrDefault(stopSchedule => stopdIds.Contains(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop LastMatchingStop(HashSet<string> stopdIds)
    {
        var scheduledStop = ScheduledStops.LastOrDefault(stopSchedule => stopdIds.Contains(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public DateTime GetStopDepartureTime(string id)
    {
        var departureTime = ScheduledStops.FirstOrDefault(stopSchedule => stopSchedule.StopId.Equals(id))?.DepartureTime ??
                            throw new ScheduledStopNotFoundException();

        return departureTime;
    }

    public void UpdateScheduledStop(string stopId, DateTime departureTime)
    {
        var stop = ScheduledStops.FirstOrDefault(scheduledStop => scheduledStop.StopId.Equals(stopId)) ?? throw new ScheduledStopNotFoundException();

        var updated = stop.UpdateDepartureTime(departureTime);

        if (updated)
            RaiseDomainEvent(new TripScheduledStopsUpdated(Id, new HashSet<string>(){ stop.Id }));
    }

    public void UpdateScheduledStops(IEnumerable<(string stopId, DateTime schedule)> stops)
    {
        var firstNewStop = stops.First();

        var newStops = ScheduledStops.TakeWhile(scheduledStop => scheduledStop.DepartureTime < firstNewStop.schedule).ToList();

        newStops.AddRange(stops.Select(x => new ScheduledStop(Guid.NewGuid().ToString(), x.stopId, x.schedule)));

        ScheduledStops.Clear();

        ScheduledStops.AddRange(newStops);

        RaiseDomainEvent(new TripScheduledStopsUpdated(Id, ScheduledStops.Select(st => st.Id).ToHashSet()));
    }
}