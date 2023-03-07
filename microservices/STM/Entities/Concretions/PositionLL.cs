using Entities.Domain;

namespace Entities.Concretions;

public struct PositionLL : IPosition
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

}