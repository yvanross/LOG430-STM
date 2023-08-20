using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal abstract class TrackingStrategy
{
    private readonly IDatetimeProvider _datetimeProvider;

    private protected readonly DateTime TrackingStartedTime;

    internal TrackingStrategy(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime)
    {
        _datetimeProvider = datetimeProvider;
        TrackingStartedTime = trackingStartedTime;
    }

    protected internal abstract string GetMessage(
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex);

    protected internal int EstimatedTimeToArrival(DateTime target) => Math.Abs(Convert.ToInt32(DeltaTime(target).TotalSeconds));

    protected internal double GetDuration() => Convert.ToInt32(DeltaTime(TrackingStartedTime).Microseconds);

    private protected double GetProgression(double currentStopIndex, double firstStopIndex, double targetStopIndex)
    {
        var progression = (currentStopIndex - firstStopIndex) / (targetStopIndex - firstStopIndex);

        return progression;
    }

    private protected TimeSpan DeltaTime(DateTime dateTime) => _datetimeProvider.GetCurrentTime() - dateTime;
}