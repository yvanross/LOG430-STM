using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class BeforeDepartureTracking : TrackingStrategy
{
    private readonly int _firstRecordedStopIndex;
    private readonly int _targetStopIndex;

    public BeforeDepartureTracking(
        IDatetimeProvider datetimeProvider,
        DateTime trackingStartedTime,
        RideUpdateInfo rideUpdateInfo)
        : base(
            datetimeProvider,
            trackingStartedTime,
            rideUpdateInfo.CurrentStopIndex,
            rideUpdateInfo.BusName)
    {
        _firstRecordedStopIndex = rideUpdateInfo.FirstRecordedStopIndex;
        _targetStopIndex = rideUpdateInfo.FirstStopIndex;
    }

    protected internal override string GetMessage()
    {
        return
        $"""
        Tracking of bus {BusName} started at {DatetimeProvider.GetMontrealTime(TrackingStartedTime).ToShortTimeString()}. It has not yet reached the first stop.
        The {BusName} has crossed {CurrentStopIndex - _firstRecordedStopIndex} stops in {Convert.ToInt32(DeltaTime(TrackingStartedTime).TotalSeconds)} seconds. 
        Making it {Convert.ToInt32(GetProgression(CurrentStopIndex, _firstRecordedStopIndex, _targetStopIndex) * 100)}% closer to the pick up stop.
        """;
    }
}