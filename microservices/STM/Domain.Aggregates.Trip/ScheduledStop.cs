using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;

namespace Domain.Aggregates.Trip;

public class ScheduledStop : Entity<ScheduledStop>
{
    public ScheduledStop(string id, string stopId, TimeSpan scheduledDepartureTime)
    {
        Id = id;
        StopId = stopId;
        ScheduledDepartureTime = scheduledDepartureTime;
    }

    public string StopId { get; internal set; }

    //Timespan in UTC time from midnight at which the bus is scheduled to depart
    public TimeSpan ScheduledDepartureTime { get; internal set; }

    public DateTime GetDepartureTime(IDatetimeProvider datetimeProvider)
    {
        // Base datetime is set to today's date with the scheduled departure time added
        var dateTime = datetimeProvider.GetCurrentTime().Date.Add(ScheduledDepartureTime);

        // If the ScheduledDepartureTime is less than the current time, assume it is for the next day
        if (ScheduledDepartureTime < datetimeProvider.GetCurrentTime().TimeOfDay)
        {
            dateTime = dateTime.AddDays(1);
        }

        return dateTime;
    }

}