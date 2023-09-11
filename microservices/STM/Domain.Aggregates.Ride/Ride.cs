using Domain.Aggregates.Ride.Events;
using Domain.Aggregates.Ride.Strategy;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;

namespace Domain.Aggregates.Ride;

public sealed class Ride : Aggregate<Ride>
{
    private TrackingStrategy? _trackingStrategy;

    internal Ride(string id, string busId, string firstRecordedStopId, string departureId, string destinationId) : base(id)
    {
        BusId = busId;
        FirstRecordedStopId = firstRecordedStopId;
        DepartureId = departureId;
        DestinationId = destinationId;
    }

    public string BusId { get; }

    public string DepartureId { get; }

    public string DestinationId { get; }

    public string FirstRecordedStopId { get; }

    public bool TrackingComplete { get; private set; }

    public DateTime TripBegunTime { get; private set; }

    public DateTime? DepartureReachedTime { get; private set; }

    public void UpdateRide(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        if(TrackingComplete) throw new InvalidOperationException("Tracking is already complete");

        TryUpdateDepartureReachedTime(rideUpdateInfo, datetimeProvider);

        ValidateUpdateState(rideUpdateInfo);

        UpdateTrackingStrategy(rideUpdateInfo, datetimeProvider);

        var message = _trackingStrategy!.GetMessage();

        TrackingComplete = rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.TargetStopIndex;

        var trackingUpdatedEvent = new RideTrackingUpdated(message, TrackingComplete, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);
    }

    public void CompleteTracking(IDatetimeProvider datetimeProvider)
    {
        TrackingComplete = true;

        var trackingUpdatedEvent = new RideTrackingUpdated("Tracking completed by exception, see logs for more info", TrackingComplete, (datetimeProvider.GetCurrentTime() - TripBegunTime).TotalSeconds);

        RaiseDomainEvent(trackingUpdatedEvent);
    }

    internal void SetTripBegunTime(DateTime tripBegunTime) => TripBegunTime = tripBegunTime;

    private void TryUpdateDepartureReachedTime(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        if (rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.FirstStopIndex)
        {
            DepartureReachedTime ??= datetimeProvider.GetCurrentTime();
        }
    }

    private void ValidateUpdateState(RideUpdateInfo rideUpdateInfo)
    {
        if (rideUpdateInfo.CurrentStopIndex < 0)
        {
            throw new AggregateInvalideStateException($"Current stop index was less than 0, aggregate is in an invalid state\n{rideUpdateInfo}");
        }

        if (rideUpdateInfo.CurrentStopIndex < rideUpdateInfo.FirstStopIndex && DepartureReachedTime is not null)
        {
            throw new AggregateInvalideStateException($"Departure reached time was not null but according to CurrentStopIndex it was never reached. Aggregate is in an invalid state \n{rideUpdateInfo}");
        }
    }

    private void UpdateTrackingStrategy(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        _trackingStrategy = TrackingStrategyFactory.Create(rideUpdateInfo, datetimeProvider, TripBegunTime, DepartureReachedTime);
    }
}