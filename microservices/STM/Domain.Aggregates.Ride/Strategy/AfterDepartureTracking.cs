using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class AfterDepartureTracking : TrackingStrategy
{
    private readonly DateTime _crossedFirstStopTime;

    public AfterDepartureTracking(IDatetimeProvider datetimeProvider, DateTime trackingStartedTime, DateTime crossedFirstStopTime, int currentStopIndex, int firstStopIndex, int targetStopIndex) :
        base(datetimeProvider, trackingStartedTime, currentStopIndex, firstStopIndex, targetStopIndex)
    {
        _crossedFirstStopTime = crossedFirstStopTime;
    }

    protected internal override string GetMessage()
    {
        return
            $"""
                Tracking started at {TrackingStartedTime.AddHours(-_datetimeProvider.GetUtcDifference())} and the bus crossed the first stop at {_crossedFirstStopTime}.
                The bus is currently at stop {_currentStopIndex} of {_targetStopIndex}.
                {Convert.ToInt32(GetProgression(_currentStopIndex, _firstStopIndex, _targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds
            """;
    }

   
}