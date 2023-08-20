using Domain.Aggregates.Ride.Strategy;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Entities;
using Domain.Events.AggregateEvents.Ride;

namespace Domain.Aggregates.Ride;

public sealed class Ride : Aggregate<Ride>
{
    public string BusId { get; set; }

    public string DepartureId { get; internal set; }

    public string DestinationId { get; internal set; }

    public string? PreviousStopId { get; internal set; }

    public bool ReachedDepartureStop { get; internal set; }

    public DateTime TripBegunTime { get; internal set; }

    public DateTime DepartureReachedTime { get; internal set; }

    private TrackingStrategy? _trackingStrategy;

    public Ride(string id, string busId, string departureId, string destinationId)
    {
        Id = id;
        BusId = busId;
        DepartureId = departureId;
        DestinationId = destinationId;
    }

    public override Ride Clone()
    {
        return new Ride(Id, BusId, DepartureId, DestinationId);
    }

    public void UpdateRide(ScheduledStop previousStop, int currentStopIndex, int firstStopIndex, int targetStopIndex, IDatetimeProvider datetimeProvider)
    {
        UpdatePreviousStop();

        if (_trackingStrategy is null)
            _trackingStrategy = new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime);
        

        if (_trackingStrategy is BeforeDepartureTracking && ReachedDepartureStop)
            _trackingStrategy = new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime);

        var targetStop = ReachedDepartureStop ? DestinationId : DepartureId;

        var message = _trackingStrategy.GetMessage(currentStopIndex, firstStopIndex, targetStopIndex, targetStop);

        var trackingCompleted = currentStopIndex >= targetStopIndex;

        var trackingUpdatedEvent = new BusTrackingUpdated(message, trackingCompleted, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);

        void UpdatePreviousStop()
        {
            if (PreviousStopId?.Equals(previousStop.StopId) is false)
            {
                PreviousStopId = previousStop;

                if (PreviousStopId.Equals(DepartureId))
                {
                    ReachedDepartureStop = true;
                }
            }
        }
    }
}