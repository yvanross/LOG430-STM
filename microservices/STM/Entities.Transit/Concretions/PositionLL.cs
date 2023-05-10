using Entities.Common.Interfaces;

namespace Entities.Transit.Concretions;

public struct PositionLL : IPosition
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

}