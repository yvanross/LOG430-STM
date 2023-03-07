using Entities.Domain;

namespace Entities.Concretions;

public class Bus : IBus
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int StopIndexAtComputationTime { get; set; }
    public IPosition Position { get; set; }
    public ITripSTM Trip { get; set; }
}