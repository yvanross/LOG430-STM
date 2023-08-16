namespace Infrastructure2.Gtfs;

public class GtfsInfo
{
    public readonly Dictionary<string, string> Info = new();

    public string? GetValue(string tag)
    {
        Info.TryGetValue(tag, out var value);
        return value;
    }
}