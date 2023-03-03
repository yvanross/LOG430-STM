using Entities.Domain;

namespace Entities.Concretions;

public class StopSTM : IStopSTM
{
    public string Id { get; init; }
    
    public IPosition Position { get; init; }

    public string Message { get; set; } = string.Empty;

    public object Clone()
    {
        return new StopSTM()
        {
            Id = Id,
            Position = (IPosition)Position.Clone(),
            Message = Message
        };
    }
}