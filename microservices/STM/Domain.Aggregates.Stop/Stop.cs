namespace Domain.Aggregates.Stop;

public class Stop : Aggregate<Stop>
{
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