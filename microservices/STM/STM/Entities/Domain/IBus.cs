
using Entities.Domain;

namespace STM.Entities.Domain;

public interface IBus
{
    string ID { get; set; }

    string Name { get; set; }

    int currentStopIndex { get; set; }

    IPosition Position { get; set; }

    ITripSTM Trip { get; set; }
}