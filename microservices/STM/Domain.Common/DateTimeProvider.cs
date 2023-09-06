using Domain.Common.Interfaces;

namespace Domain.Common;

public class DateTimeProvider : IDatetimeProvider
{
    public DateTime GetCurrentTime()
    {
        return DateTime.UtcNow;
    }

    public int GetUtcDifference()
    {
        var currentUtcDateTime = DateTime.UtcNow;

        var utcMinusFourOffset = TimeSpan.FromHours(-4);

        var utcMinusFourTimeZone = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(timeZone => timeZone.BaseUtcOffset.Equals(utcMinusFourOffset));

        var isDaylightSavingTime = utcMinusFourTimeZone?.IsDaylightSavingTime(currentUtcDateTime) ?? false;

        return isDaylightSavingTime ? 5 : 4;
    }
}