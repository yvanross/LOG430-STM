using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.Common.Seedwork.Abstract;
using Domain.Common.Seedwork.Interfaces;
using Domain.Entities;

namespace Domain.Aggregates;

public class Ride : Aggregate<Ride>
{
    public string Id { get; private set; }

    public string BusId { get; private set; }

    public ScheduledStop Departure { get; private set; }

    public ScheduledStop Destination { get; private set; }

    public Ride(string id, string busId, ScheduledStop departure, ScheduledStop destination)
    {
        Id = id;
        BusId = busId;
        Departure = departure;
        Destination = destination;
    }

    public override Ride Clone()
    {
        return new Ride(Id, BusId, Departure.Clone(), Destination.Clone());
    }

    public bool IsTimeRelevant(IDatetimeProvider datetimeProvider)
    {
        return Departure.DepartureTime > datetimeProvider.GetCurrentTime() &&
               Destination.DepartureTime > datetimeProvider.GetCurrentTime() &&
               Departure.DepartureTime < Destination.DepartureTime;
    }

    public double GetExpectedTimeOfArrivalInSecond(IDatetimeProvider datetimeProvider)
    {
        return (Departure.DepartureTime - datetimeProvider.GetCurrentTime()).TotalSeconds;
    }

    public bool Equals(Ride? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && BusId == other.BusId && Departure.Equals(other.Departure) && Destination.Equals(other.Destination);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Ride)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, BusId, Departure, Destination);
    }

    public void Track()
    {
        // publish tracking event and event handler should begin co-routine to provide updates on it
    }
}