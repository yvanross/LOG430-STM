namespace Entities.Domain;

public interface IPosition
{
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    double Longitude { get; set; }
}