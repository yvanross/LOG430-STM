namespace ApplicationLogic.Helpers;

public class TimezoneHelper
{
    public static int UtcDiff
    {
        get
        {
            var currentUtcDateTime = DateTime.UtcNow;

            var utcMinusFourOffset = TimeSpan.FromHours(-4);

            var utcMinusFourTimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(timeZone => timeZone.BaseUtcOffset.Equals(utcMinusFourOffset));

            var isDaylightSavingTime = utcMinusFourTimeZone?.IsDaylightSavingTime(currentUtcDateTime) ?? false;

            return isDaylightSavingTime ? 5 : 4;
        }
    }
}