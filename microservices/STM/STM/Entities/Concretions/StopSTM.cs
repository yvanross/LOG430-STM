using Entities.Domain;
using STM.Entities.Domain;

namespace STM.Entities.Concretions;

public class StopSTM : IStopSTM
{
    public string ID { get; init; }
    
    public IPosition Position { get; init; }

    public string Message { get; set; }

    public object Clone()
    {
        return new StopSTM()
        {
            ID = ID,
            Position = (IPosition)Position.Clone(),
            Message = Message
        };
    }
}