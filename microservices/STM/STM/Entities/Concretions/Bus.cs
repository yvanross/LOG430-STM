using Entities.Domain;
using STM.Entities.Domain;

namespace STM.Entities.Concretions;

public class Bus : IBus
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int currentStopIndex { get; set; }
    public IPosition Position { get; set; }
    public ITripSTM Trip { get; set; }
}