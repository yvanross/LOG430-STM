using Domain.Common.Seedwork.Interfaces;
using Domain.ValueObjects;
using System.Runtime.CompilerServices;
using Domain.Common.Seedwork.Abstract;

[assembly: InternalsVisibleTo("Domain.Entities")]
namespace Domain.Entities;

public class Bus : Aggregate<Bus>
{
    public string Name { get; private set; }

    public string TripId { get; private set; }

    public int CurrentStopIndex { get; private set; }

    public Bus(string id, string name, string tripId, int currentStopIndex)
    {
        Id = id;
        Name = name;
        TripId = tripId;
        CurrentStopIndex = currentStopIndex;
    }

    public override Bus Clone()
    {
        return new Bus(Id, Name, TripId, CurrentStopIndex);
    }
}