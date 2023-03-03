using Entities.Domain;

namespace StaticGTFS.Concretions;

public class Stop : IStop
{
    public string ID { get; init; }
    
    public IPosition Position { get; init; }

    public object Clone()
    {
        return new Stop()
        {
            ID = ID,
            Position = (IPosition)Position.Clone(),
        };
    }
}