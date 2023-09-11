using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class AtDepartureTracking : TrackingStrategy
{
    private readonly DateTime _crossedFirstStopTime;

    public AtDepartureTracking(
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
        if(crossedFirstStopTime.HasValue == false) throw new Exception("Crossed first stop time cannot be null since the departure stop has been reached");

        _crossedFirstStopTime = crossedFirstStopTime.Value;
    }

    protected internal override string GetMessage()
    {
        return
        $"""
        Bus {BusName} is at the the pick up stop. It took {Convert.ToInt32((_crossedFirstStopTime - TrackingStartedTime).TotalSeconds)} seconds.
        Tracking towards destination will begin shortly.
        """;
    }
}