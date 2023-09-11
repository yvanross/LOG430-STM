using Domain.Common.Interfaces;

namespace Domain.Services.Utility;

public class TimeServices
{
    private readonly IDatetimeProvider _datetimeProvider;

    public TimeServices(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    /// <summary>
    /// Returns UTC datetime from seconds since epoch in UTC-4 (includes daylight savings)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public DateTime LongToDatetime(long time)
    {
        var datetime = DateTime.UnixEpoch.AddSeconds(time);

        datetime = datetime.AddHours(_datetimeProvider.GetUtcDifference());

        return datetime;
    }
}