using Domain.Aggregates.Ride.Strategy;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Ride;

namespace Domain.Aggregates.Ride;

public sealed class Ride : Aggregate<Ride>
{
    private TrackingStrategy? _trackingStrategy;

    public Ride(string id, string busId, string firstRecordedStopId, string departureId, string destinationId)
    {
        Id = id;
        BusId = busId;
        FirstRecordedStopId = firstRecordedStopId;
        DepartureId = departureId;
        DestinationId = destinationId;
    }

    public string BusId { get; set; }

    public string DepartureId { get; internal set; }

    public string DestinationId { get; internal set; }

    public string FirstRecordedStopId { get; internal set; }

    public bool ReachedDepartureStop { get; internal set; }

    public bool TrackingComplete { get; internal set; }

    public DateTime TripBegunTime { get; internal set; }

    public DateTime? DepartureReachedTime { get; internal set; }

    public void UpdateRide(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        if(TrackingComplete) throw new InvalidOperationException("Tracking is already complete");

        ValidateState(rideUpdateInfo, datetimeProvider);

        var message = _trackingStrategy!.GetMessage();

        TrackingComplete = rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.TargetStopIndex;

        var trackingUpdatedEvent = new BusTrackingUpdated(message, TrackingComplete, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);
    }

    private void ValidateState(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider)
    {
        _trackingStrategy ??= new BeforeDepartureTracking(datetimeProvider, TripBegunTime, rideUpdateInfo);

        if (_trackingStrategy is BeforeDepartureTracking && rideUpdateInfo.CurrentStopIndex >= rideUpdateInfo.FirstStopIndex)
        {
            DepartureReachedTime ??= datetimeProvider.GetCurrentTime().AddHours(-datetimeProvider.GetUtcDifference());

            _trackingStrategy = new AtDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime.Value, rideUpdateInfo);
        }
        if (_trackingStrategy is AtDepartureTracking && rideUpdateInfo.CurrentStopIndex > rideUpdateInfo.FirstStopIndex)
        {
            _trackingStrategy = new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime.Value, rideUpdateInfo);
        }
    }

    public void CompleteTracking(IDatetimeProvider datetimeProvider)
    {
        TrackingComplete = true;

        var trackingUpdatedEvent = new BusTrackingUpdated("Tracking completed by exception, see logs for more info", TrackingComplete, (datetimeProvider.GetCurrentTime() - TripBegunTime).TotalSeconds);

        RaiseDomainEvent(trackingUpdatedEvent);
    }
}