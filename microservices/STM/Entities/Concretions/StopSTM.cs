using Entities.Domain;

namespace Entities.Concretions;

public class StopSTM : IStopSTM
{
    public string Id { get; init; }
    
    public IPosition Position { get; init; }

    public string Message { get; set; } = string.Empty;
}