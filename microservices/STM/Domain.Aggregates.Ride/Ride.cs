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

    public void UpdateRide(int firstRecordedStopIndex,
        int currentStopIndex,
        int firstStopIndex,
        int targetStopIndex,
        IDatetimeProvider datetimeProvider)
    {
        if(TrackingComplete) throw new InvalidOperationException("Tracking is already complete");

        InvalidateState(currentStopIndex, firstRecordedStopIndex, firstStopIndex, targetStopIndex, datetimeProvider);

        var message = _trackingStrategy!.GetMessage();

        TrackingComplete = currentStopIndex >= targetStopIndex;

        var trackingUpdatedEvent = new BusTrackingUpdated(message, TrackingComplete, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);
    }

    private void InvalidateState(int currentStopIndex, int firstRecordedStopIndex, int firstStopIndex, int targetStopIndex, IDatetimeProvider datetimeProvider)
    {
        _trackingStrategy ??= new BeforeDepartureTracking(datetimeProvider, TripBegunTime, currentStopIndex, firstRecordedStopIndex, firstStopIndex);

        if (_trackingStrategy is BeforeDepartureTracking && currentStopIndex >= targetStopIndex)
        {
            DepartureReachedTime ??= datetimeProvider.GetCurrentTime();

            _trackingStrategy = new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime.Value, currentStopIndex, firstStopIndex, targetStopIndex);
        }
    }
}