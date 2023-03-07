namespace GTFS;

public class GTFSInfo
{
    public string? GetValue(string tag)
    {
        Info.TryGetValue(tag, out var value);
        return value;
    }

    public Dictionary<string, string> Info = new Dictionary<string, string>();
}