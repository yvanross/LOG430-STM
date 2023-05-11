using Entities.Common.Interfaces;
using Entities.Transit.Interfaces;

namespace Entities.Transit.Concretions;

public class Bus : IBus
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int StopIndexAtComputationTime { get; set; }
    public IPosition Position { get; set; }
    public ITransitTrip TransitTrip { get; set; }
}