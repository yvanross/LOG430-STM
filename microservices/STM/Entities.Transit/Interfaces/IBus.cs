using Entities.Common.Interfaces;

namespace Entities.Transit.Interfaces;

public interface IBus
{
    string Id { get; set; }

    string Name { get; set; }

    int StopIndexAtComputationTime { get; set; }

    IPosition Position { get; set; }

    ITransitTrip TransitTrip { get; set; }
}