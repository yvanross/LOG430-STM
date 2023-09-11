using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class AfterDepartureTracking : TrackingStrategy
{
    private readonly DateTime _crossedFirstStopTime;

    private readonly int _firstStopIndex;
    private readonly int _targetStopIndex;

    public AfterDepartureTracking(
        IDatetimeProvider datetimeProvider, 
        DateTime trackingStartedTime, 
        DateTime? crossedFirstStopTime, 
        RideUpdateInfo rideUpdateInfo) 
        : base(
            datetimeProvider,
            trackingStartedTime,
            rideUpdateInfo.CurrentStopIndex, 
            rideUpdateInfo.BusName)
    {
        if (crossedFirstStopTime.HasValue == false) throw new Exception("Crossed first stop time cannot be null since the departure stop has been reached");

        _crossedFirstStopTime = crossedFirstStopTime.Value;
        _firstStopIndex = rideUpdateInfo.FirstStopIndex;
        _targetStopIndex = rideUpdateInfo.TargetStopIndex;
    }

    protected internal override string GetMessage()
    {
        return
            $"""
            Tracking of bus {BusName} started at {DatetimeProvider.GetMontrealTime(TrackingStartedTime).ToShortTimeString()} and it has crossed the first stop at {DatetimeProvider.GetMontrealTime(_crossedFirstStopTime).ToShortTimeString()}.
            The {BusName} is currently at stop {CurrentStopIndex} of {_targetStopIndex}.
            It completed {Convert.ToInt32(GetProgression(CurrentStopIndex, _firstStopIndex, _targetStopIndex) * 100)}% in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds
            """;
    }


    
}