using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class BeforeDepartureTracking : TrackingStrategy
{
    public BeforeDepartureTracking(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime, int currentStopIndex, int firstStopIndex, int targetStopIndex) :
        base(datetimeProvider, trackingStartedTime, currentStopIndex, firstStopIndex, targetStopIndex)
    { }

    protected internal override string GetMessage()
    {
        return
        $"""
        Tracking started at {TrackingStartedTime}. Bus has not yet reached the first stop.
        It has crossed {_targetStopIndex - _currentStopIndex} stops, completing {Convert.ToInt32(GetProgression(_currentStopIndex, _firstStopIndex, _targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(TrackingStartedTime).TotalSeconds)} seconds
        """;
    }
}