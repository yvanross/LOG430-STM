namespace Entities.Domain;

public interface IStopSTM
{
    /// <summary>
    /// Stop TripID
    /// </summary>
    string Id { get; init; }

    /// <summary>
    /// Stop Position in Latitude Longitude
    /// </summary>
    IPosition Position { get; init; }

    public string Message { get; set; }
}
