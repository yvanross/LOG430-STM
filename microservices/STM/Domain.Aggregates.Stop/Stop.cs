using Domain.Common.Seedwork.Abstract;
using Domain.ValueObjects;

namespace Domain.Aggregates.Stop;

public class Stop : Aggregate<Stop>
{
    // for EF
    private Stop(string id) : base(id) { }

    internal Stop(string id, double latitude, double longitude) : base(id)
    {
        Position = new Position(latitude, longitude);
    }

    public Position Position { get; }
}