using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class BeforeDepartureTracking : TrackingStrategy
{
    public BeforeDepartureTracking(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime) : base(datetimeProvider, trackingStartedTime) { }

    protected internal override string GetMessage(
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex)
    {
        return
        $"""
        Tracking started at {TrackingStartedTime}.
        The bus has crossed {currentStopIndex - firstStopIndex} stops, completing {Convert.ToInt32(GetProgression(currentStopIndex, firstStopIndex, targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(TrackingStartedTime).TotalSeconds)} seconds
        """;
    }
}