using Domain.Common.Interfaces;

namespace Infrastructure.FileHandlers.StaticGtfs.Mappers.TypeConverter;

public sealed class GtfsTimespanConverter(IDatetimeProvider datetimeProvider)
{
    public TimeSpan ConvertFromString(string? text)
    {
        var HMSArray = text.Split(":");

        var hours = int.Parse(HMSArray[0]);
        var minutes = int.Parse(HMSArray[1]);
        var seconds = int.Parse(HMSArray[2]);

        var timeSpan = new TimeSpan(hours, minutes, seconds);

        //since the gtfs in local time, we need to add the utc difference to get the correct UTC time
        timeSpan = timeSpan.Add(TimeSpan.FromHours(datetimeProvider.GetUtcDifference()));

        return timeSpan;
    }
}