using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal class ReachedDestinationStop : TrackingStrategy
{
    private readonly DateTime _crossedFirstStopTime;

    public ReachedDestinationStop(
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
    }

    protected internal override string GetMessage()
    {
        return $"Bus {BusName} has reached its destination stop in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds";
    }
}