using Domain.Aggregates.Ride.Strategy;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Entities;
using Domain.Events.AggregateEvents.Ride;

namespace Domain.Aggregates.Ride;

public sealed class Ride : Aggregate<Ride>
{
    public string Id { get; }

    public string BusId { get; }

    public ScheduledStop Departure { get; }

    public ScheduledStop Destination { get; }

    public ScheduledStop? PreviousStop { get; private set; }

    public bool ReachedDepartureStop { get; private set; }

    public DateTime TripBegunTime { get; }

    public DateTime DepartureReachedTime { get; }

    private TrackingStrategy _trackingStrategy;

    private readonly IDatetimeProvider _datetimeProvider;

    public Ride(string id, string busId, ScheduledStop departure, ScheduledStop destination, IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
        Id = id;
        BusId = busId;
        Departure = departure;
        Destination = destination;

        _trackingStrategy = ReachedDepartureStop ?
            new AfterDepartureTracking(datetimeProvider, TripBegunTime, DepartureReachedTime) :
            new BeforeDepartureTracking(datetimeProvider, TripBegunTime);
    }

    public override Ride Clone()
    {
        return new Ride(Id, BusId, Departure.Clone(), Destination.Clone(), _datetimeProvider);
    }

    public bool IsTimeRelevant()
    {
        return Departure.DepartureTime > _datetimeProvider.GetCurrentTime() &&
               Destination.DepartureTime > _datetimeProvider.GetCurrentTime() &&
               Departure.DepartureTime < Destination.DepartureTime;
    }

    public void UpdateRide(ScheduledStop previousStop, int currentStopIndex, int firstStopIndex, int targetStopIndex)
    {
        UpdatePreviousStop();

        if (_trackingStrategy is BeforeDepartureTracking && ReachedDepartureStop)
            _trackingStrategy = new AfterDepartureTracking(_datetimeProvider, TripBegunTime, DepartureReachedTime);

        var targetStop = ReachedDepartureStop ? Destination : Departure;

        var message = _trackingStrategy.GetMessage(currentStopIndex, firstStopIndex, targetStopIndex, targetStop);

        var trackingCompleted = currentStopIndex >= targetStopIndex;

        var trackingUpdatedEvent = new BusTrackingUpdated(message, trackingCompleted, _trackingStrategy.GetDuration());

        RaiseDomainEvent(trackingUpdatedEvent);

        void UpdatePreviousStop()
        {
            if (PreviousStop?.Equals(previousStop) is false)
            {
                PreviousStop = previousStop;

                if (PreviousStop.Equals(Departure))
                {
                    ReachedDepartureStop = true;
                }
            }
        }
    }
}