using Domain.Common.Seedwork.Abstract;
using Domain.ValueObjects;

namespace Domain.Aggregates.Stop;

public class Stop : Aggregate<Stop>
{
    public Position Position { get; internal set; }

    // Add a private parameterless constructor
    private Stop() { }

    public Stop(string id, double latitude, double longitude)
    {
        Id = id;
        Position = new Position(latitude, longitude);
    }

    public override Stop Clone()
    {
        return new Stop(Id, Position.Latitude, Position.Longitude);
    }
}