using Domain.Aggregates.Ride.Strategy;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Ride;

namespace Domain.Aggregates.Ride;

public sealed class Ride : Aggregate<Ride>
{
    private TrackingState? _trackingStrategy;

    public Ride(string id, string busId, string firstRecordedStopId, string departureId, string destinationId)
    {
        Id = id;
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

        ValidateUpdateState(rideUpdateInfo);

        UpdateTrackingStrategy(rideUpdateInfo, datetimeProvider);

        var message = _trackingStrategy!.GetMessage();

        TrackingComplete = rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.TargetStopIndex;

        var trackingUpdatedEvent = new BusTrackingUpdated(message, TrackingComplete, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);
    }

    internal void SetTripBegunTime(DateTime tripBegunTime) => TripBegunTime = tripBegunTime;

    private void ValidateUpdateState(RideUpdateInfo rideUpdateInfo)
    {
        if (rideUpdateInfo.CurrentStopIndex < 0)
        {
            throw new AggregateInvalideStateException("Current stop index was less than 0, aggregate is in an invalid state");
        }

        if (rideUpdateInfo.CurrentStopIndex < rideUpdateInfo.FirstStopIndex && DepartureReachedTime is not null)
        {
            throw new AggregateInvalideStateException("Departure reached time was not null but according to CurrentStopIndex it was never reached. Aggregate is in an invalid state");
        }
    }

    private void UpdateTrackingStrategy(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        //Argument to use a finite state machine,
        //but it would be overkill for this simple case, especially since the state changes are only forward.
        //It would be more code duplication and object creation than it's worth

        if (rideUpdateInfo.CurrentStopIndex < rideUpdateInfo.FirstStopIndex)
        {
            _trackingStrategy = new BeforeDepartureTracking(datetimeProvider, TripBegunTime, rideUpdateInfo);
        }
        else if (rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.FirstStopIndex)
        {
            DepartureReachedTime ??= datetimeProvider.GetCurrentTime().AddHours(-datetimeProvider.GetUtcDifference());

            _trackingStrategy = new AtDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime.Value, rideUpdateInfo);
        }
        else if (rideUpdateInfo.CurrentStopIndex > rideUpdateInfo.FirstStopIndex)
        {
            _trackingStrategy = new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime.Value, rideUpdateInfo);
        }

        if (_trackingStrategy is null)
        {
            throw new AggregateInvalideStateException("Tracking strategy was null, aggregate is in an invalid state");
        }
    }

    public void CompleteTracking(IDatetimeProvider datetimeProvider)
    {
        TrackingComplete = true;

        var trackingUpdatedEvent = new BusTrackingUpdated("Tracking completed by exception, see logs for more info", TrackingComplete, (datetimeProvider.GetCurrentTime() - TripBegunTime).TotalSeconds);

        RaiseDomainEvent(trackingUpdatedEvent);
    }
}