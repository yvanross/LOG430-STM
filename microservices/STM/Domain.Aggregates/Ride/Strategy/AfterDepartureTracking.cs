using Domain.Common.Interfaces;
using Domain.Entities;

namespace Domain.Aggregates.Ride.Strategy;

internal class AfterDepartureTracking : TrackingStrategy
{
    private readonly DateTime _crossedFirstStopTime;

    public AfterDepartureTracking(
        IDatetimeProvider datetimeProvider,
        DateTime trackingStartedTime,
        DateTime crossedFirstStopTime) : base(datetimeProvider, trackingStartedTime)
    {
        _crossedFirstStopTime = crossedFirstStopTime;
    }

    protected internal override string GetMessage(
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex,
        ScheduledStop targetStop)
    {
        return
            $"""
                Tracking started at {TrackingStartedTime} and the bus crossed the first stop at {_crossedFirstStopTime}.
                The bus is currently at stop {currentStopIndex} of {targetStopIndex}.
                Expecting it to arrive at destination in {EstimatedTimeToArrival(targetStop.DepartureTime)} seconds.
                {Convert.ToInt32(GetProgression(currentStopIndex, firstStopIndex, targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds
            """;
    }
}