namespace Entities.Domain;

public interface IStop : ICloneable
{
    /// <summary>
    /// Stop TripID
    /// </summary>
    string ID { get; init; }

    /// <summary>
    /// Stop Position in Latitude Longitude
    /// </summary>
    IPosition Position { get; init; }
}