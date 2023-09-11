using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.Gtfs.Wrappers;

public struct StopWrapper : IStopWrapper
{
    public StopWrapper(GtfsInfo info)
    {
        Id = GetId(info);
        Longitude = GetLongitude(info);
        Latitude = GetLatitude(info);

        info.Dispose();
    }

    public string Id { get; }

    public double Longitude { get; }

    public double Latitude { get; }

    private string GetId(GtfsInfo gtfsInfo)
    {
        return gtfsInfo.GetValue("stop_id");
    }

    private double GetLongitude(GtfsInfo gtfsInfo)
    {
        if (double.TryParse(gtfsInfo.GetValue("stop_lon"), out var longitude) is false)
            throw new ArgumentException("Longitude is not a double");

        return longitude;
    }

    private double GetLatitude(GtfsInfo gtfsInfo)
    {
        if (double.TryParse(gtfsInfo.GetValue("stop_lat"), out var latitude) is false)
            throw new ArgumentException("Latitude is not a double");

        return latitude;
    }
}