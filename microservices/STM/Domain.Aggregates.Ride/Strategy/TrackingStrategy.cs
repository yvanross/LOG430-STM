using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal abstract class TrackingStrategy
{
    private protected readonly IDatetimeProvider DatetimeProvider;

    private protected readonly DateTime TrackingStartedTime;
    private protected readonly int CurrentStopIndex;
    private protected readonly string BusName;

    protected TrackingStrategy(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime, int currentStopIndex, string busName)
    {
        DatetimeProvider = datetimeProvider;
        TrackingStartedTime = trackingStartedTime;
        CurrentStopIndex = currentStopIndex;
        BusName = busName;
    }

    protected internal abstract string GetMessage();

    protected internal double GetDuration()
    {
        return DeltaTime(TrackingStartedTime).TotalMicroseconds;
    }

    private protected double GetProgression(double currentStopIndex, double firstStopIndex, double targetStopIndex)
    {
        var progression = (currentStopIndex - firstStopIndex) / (targetStopIndex - firstStopIndex);

        return progression;
    }

    private protected TimeSpan DeltaTime(DateTime dateTime)
    {
        return DatetimeProvider.GetCurrentTime() - dateTime;
    }

}