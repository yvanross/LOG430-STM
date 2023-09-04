using Domain.Common.Seedwork.Abstract;
using Domain.ValueObjects;

namespace Domain.Aggregates.Stop;

public class Stop : Aggregate<Stop>
{
    //for EF
    private Stop() { }

    public Stop(string id, double latitude, double longitude)
    {
        Id = id;
        Position = new Position(latitude, longitude);
    }

    public Position Position { get; internal set; }
}