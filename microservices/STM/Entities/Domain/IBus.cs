
namespace Entities.Domain;

public interface IBus
{
    string Id { get; set; }

    string Name { get; set; }

    int currentStopIndex { get; set; }

    IPosition Position { get; set; }

    ITripSTM Trip { get; set; }
}