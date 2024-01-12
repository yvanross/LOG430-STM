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

        var utcMinusFourTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        var isDaylightSavingTime = utcMinusFourTimeZone.IsDaylightSavingTime(currentUtcDateTime);

        return isDaylightSavingTime ? 4 : 5;
    }

    public DateTime GetMontrealTime()
    {
        return DateTime.UtcNow.AddHours(-GetUtcDifference());
    }

    public DateTime GetMontrealTime(DateTime from)
    {
        return from.AddHours(-GetUtcDifference());
    }
}