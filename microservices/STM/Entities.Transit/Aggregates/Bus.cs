using Entities.Common.ValueObjects;
using Entities.Transit.Entities;

namespace Entities.Transit.Aggregates;

public class Bus
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public int StopIndexAtComputationTime { get; set; }

    public required Position Position { get; set; }

    public required TransitTrip TransitTrip { get; set; }
}