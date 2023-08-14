using Domain.Common.Seedwork.Abstract;
using Domain.Common.Seedwork.Interfaces;
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

    public bool Equals(Stop? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Position.Equals(other.Position);
    }

    public override Stop Clone()
    {
        return new Stop(Id, Position);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}