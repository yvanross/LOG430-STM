using Entities.Domain;

namespace STM.Entities.Domain;

public interface IStopSTM : IStop
{
    /// <summary>
    /// Stop TripID
    /// </summary>
    string ID { get; init; }

    /// <summary>
    /// Stop Position in Latitude Longitude
    /// </summary>
    IPosition Position { get; init; }

    public string Message { get; set; }

}
