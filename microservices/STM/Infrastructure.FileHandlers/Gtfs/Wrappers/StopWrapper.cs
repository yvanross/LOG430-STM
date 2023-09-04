using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public sealed class StopWrapper : IStopWrapper
{
    private readonly GtfsInfo _info;

    public StopWrapper(GtfsInfo info)
    {
        _info = info;

        Id = GetId();
        Longitude = GetLongitude();
        Latitude = GetLatitude();
    }

    public string Id { get; }

    public double Longitude { get; }

    public double Latitude { get; }

    private string GetId()
    {
        return _info.GetValue("stop_id");
    }

    private double GetLongitude()
    {
        if (double.TryParse(_info.GetValue("stop_lon"), out var longitude) is false)
            throw new ArgumentException("Longitude is not a double");

        return longitude;
    }

    private double GetLatitude()
    {
        if (double.TryParse(_info.GetValue("stop_lat"), out var latitude) is false)
            throw new ArgumentException("Latitude is not a double");

        return latitude;
    }
}