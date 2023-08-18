using Domain.Common.Interfaces;
using Domain.Entities;

namespace Domain.Aggregates.Ride.Strategy;

internal class BeforeDepartureTracking : TrackingStrategy
{
    public BeforeDepartureTracking(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime) : base(datetimeProvider, trackingStartedTime) { }

    protected internal override string GetMessage(
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex,
        ScheduledStop targetStop)
    {
        return
        $"""
        Tracking started at {TrackingStartedTime} and the bus is approximately {EstimatedTimeToArrival(targetStop.DepartureTime)} seconds from the first stop.
        The bus has crossed {currentStopIndex - firstStopIndex} stops, completing {Convert.ToInt32(GetProgression(currentStopIndex, firstStopIndex, targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(TrackingStartedTime).TotalSeconds)} seconds
        """;
    }
}