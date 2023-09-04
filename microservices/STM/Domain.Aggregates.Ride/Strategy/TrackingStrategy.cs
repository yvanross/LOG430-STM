using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal abstract class TrackingStrategy
{
    private readonly IDatetimeProvider _datetimeProvider;

    private protected readonly DateTime TrackingStartedTime;
    private protected readonly int _currentStopIndex;
    private protected readonly int _firstStopIndex;
    private protected readonly int _targetStopIndex;

    internal TrackingStrategy(
        IDatetimeProvider datetimeProvider, 
        DateTime trackingStartedTime,
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex)
    {
        _datetimeProvider = datetimeProvider;
        TrackingStartedTime = trackingStartedTime;
        _currentStopIndex = currentStopIndex;
        _firstStopIndex = firstStopIndex;
        _targetStopIndex = targetStopIndex;
    }

    protected internal abstract string GetMessage();

    protected internal int EstimatedTimeToArrival(DateTime target)
    {
        return Math.Abs(Convert.ToInt32(DeltaTime(target).TotalSeconds));
    }

    protected internal double GetDuration()
    {
        return Convert.ToInt32(DeltaTime(TrackingStartedTime).Microseconds);
    }

    private protected double GetProgression(double currentStopIndex, double firstStopIndex, double targetStopIndex)
    {
        var progression = (currentStopIndex - firstStopIndex) / (targetStopIndex - firstStopIndex);

        return progression;
    }

    private protected TimeSpan DeltaTime(DateTime dateTime)
    {
        return _datetimeProvider.GetCurrentTime() - dateTime;
    }
}