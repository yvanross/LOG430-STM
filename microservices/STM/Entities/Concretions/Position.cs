using Entities.Domain;

namespace Entities.Concretions;

public struct Position : IPosition
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}