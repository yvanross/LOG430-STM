using Domain.Common.Seedwork.Abstract;

namespace Domain.Aggregates;

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