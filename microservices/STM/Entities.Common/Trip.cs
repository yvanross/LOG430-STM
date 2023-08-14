using System.Collections.Immutable;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Common.Seedwork.Interfaces;
using Domain.Entities;

namespace Domain.Aggregates;

public class Trip : Aggregate<Trip>
{
    public string Id { get; private set; }

    internal ImmutableList<ScheduledStop> StopSchedules { get; }

    public Trip(IEnumerable<ScheduledStop> stopSchedules, string id)
    {
        StopSchedules = stopSchedules.ToImmutableList();
        Id = id;
    }

    public bool Equals(Trip? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && StopSchedules.SequenceEqual(other.StopSchedules);
    }

    public override Trip Clone()
    {
        return new Trip(StopSchedules.Select(x => x.Clone()).ToList(), Id);
    }

    public bool IsTimeRelevant(IDatetimeProvider datetimeProvider)
    {
        if (StopSchedules.Any() is false) throw new TripHasNoScheduledStopException();

        var lastStopTime = DeltaHours(StopSchedules[^1].DepartureTime);

        return lastStopTime > 0;

        double DeltaHours(DateTime from)
        {
            var minutes = (from - datetimeProvider.GetCurrentTime()).TotalMinutes;

            return minutes;
        }
    }

    public bool ContainsStop(string stopId)
    {
        return StopSchedules.Any(stopSchedule => stopSchedule.StopId.Equals(stopId));
    }

    public int NumberOfStops() => StopSchedules.Count;

    public ScheduledStop GetStopByIndex(int index) => StopSchedules[index].Clone();

    public DateTime GetStopDepartureTime(string id)
    {
        return StopSchedules.FirstOrDefault(stopSchedule => stopSchedule.Id.Equals(id))?.DepartureTime ??
               throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop FirstMatchingStop(Dictionary<string, Stop> stops)
    {
        var scheduledStop = StopSchedules.FirstOrDefault(stopSchedule => stops.ContainsKey(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }

    public ScheduledStop LastMatchingStop(Dictionary<string, Stop> stops)
    {
        var scheduledStop = StopSchedules.LastOrDefault(stopSchedule => stops.ContainsKey(stopSchedule.StopId));

        return scheduledStop ?? throw new ScheduledStopNotFoundException();
    }
}