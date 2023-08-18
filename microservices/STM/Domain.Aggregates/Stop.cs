using Domain.Common.Seedwork.Abstract;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public class Stop : Aggregate<Stop>
{
    public string Id { get; private set; }

    public Position Position { get; private set; }

    public Stop(string id, Position position)
    {
        Id = id;
        Position = position;
    }

    public override Stop Clone()
    {
        return new Stop(Id, Position);
    }
}