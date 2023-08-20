using System.Collections.Immutable;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Trip;

namespace Domain.Aggregates.Trip;

public class Trip : Aggregate<Trip>
{
    //risk of race condition
    internal List<ScheduledStop> ScheduledStops { get; set; }

    private readonly ReaderWriterLockSlim _lock = new();

    private Trip(){}

    public Trip(string id, List<ScheduledStop> scheduledStops)
    {
        ScheduledStops = scheduledStops;
        Id = id;
    }

    internal static Trip CreateTrip(string tripId, IEnumerable<(string stopId, DateTime schedule)> stopSchedules)
    {
        return new Trip(tripId, 
            stopSchedules.Select(x => new ScheduledStop( Guid.NewGuid().ToString(), x.stopId, x.schedule)).ToList());
    }

    public override Trip Clone()
    {
        return new Trip(Id, ScheduledStops.Select(x => x.Clone()).ToList());
    }

    public bool IsTimeRelevant(IDatetimeProvider datetimeProvider)
    {
        _lock.EnterReadLock();

        if (ScheduledStops.Any() is false) throw new TripHasNoScheduledStopException();

        var lastStopTime = DeltaHours(ScheduledStops[^1].DepartureTime);

        _lock.ExitReadLock();

        return lastStopTime > 0;

        double DeltaHours(DateTime from)
        {
            var minutes = (from - datetimeProvider.GetCurrentTime()).TotalMinutes;

            return minutes;
        }
    }

    public bool ContainsStop(string stopId)
    {
        _lock.EnterReadLock();

        var condition = ScheduledStops.Any(stopSchedule => stopSchedule.StopId.Equals(stopId));

        _lock.ExitReadLock();

        return condition;
    }

    public int NumberOfStops()
    {
        _lock.EnterReadLock();

        var count = ScheduledStops.Count;

        _lock.ExitReadLock();

        return count;
    }

    public string GetStopIdByIndex(int index)
    {
        _lock.EnterReadLock();

        var id = ScheduledStops[index].StopId;

        _lock.ExitReadLock();

        return id;
    }

    public string GetScheduleIdByStopId(string stopId)
    {
        _lock.EnterReadLock();

        var id = ScheduledStops.FirstOrDefault(scheduledStop => scheduledStop.StopId.Equals(stopId))?.Id ?? 
                 throw new ScheduledStopNotFoundException();

        _lock.ExitReadLock();

        return id;
    }

    public int GetIndexOfStop(string stopId)
    {
        _lock.EnterReadLock();

        var index = ScheduledStops.FindIndex(stop => stop.StopId.Equals(stopId));

        _lock.ExitReadLock();

        if (index == -1) throw new ScheduledStopNotFoundException();

        return index;
    }

    public ScheduledStop FirstMatchingStop(HashSet<string> stopdIds)
    {
        _lock.EnterReadLock();

        var scheduledStop = ScheduledStops.FirstOrDefault(stopSchedule => stopdIds.Contains(stopSchedule.StopId));

        _lock.ExitReadLock();

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop LastMatchingStop(HashSet<string> stopdIds)
    {
        _lock.EnterReadLock();

        var scheduledStop = ScheduledStops.LastOrDefault(stopSchedule => stopdIds.Contains(stopSchedule.StopId));

        _lock.ExitReadLock();

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public DateTime GetStopDepartureTime(string id)
    {
        _lock.EnterReadLock();

        var departureTime = ScheduledStops.FirstOrDefault(stopSchedule => stopSchedule.Id.Equals(id))?.DepartureTime ??
                            throw new ScheduledStopNotFoundException();

        _lock.ExitReadLock();

        return departureTime;
    }

    public void UpdateScheduledStop(string stopId, DateTime departureTime)
    {
        _lock.EnterReadLock();

        var stop = ScheduledStops.FirstOrDefault(scheduledStop => scheduledStop.StopId.Equals(stopId)) ?? throw new ScheduledStopNotFoundException();

        var updated = stop.UpdateDepartureTime(departureTime);

        if (updated)
            RaiseDomainEvent(new TripScheduledStopsUpdated(Id));

        _lock.ExitReadLock();
    }

    public void UpdateScheduledStops(IEnumerable<(string stopId, DateTime schedule)> stops)
    {
        _lock.EnterWriteLock();
       
        ScheduledStops = stops.Select(x => new ScheduledStop(Guid.NewGuid().ToString(), x.stopId, x.schedule)).ToList();

        RaiseDomainEvent(new TripScheduledStopsUpdated(Id));

        _lock.ExitWriteLock();

    }
}