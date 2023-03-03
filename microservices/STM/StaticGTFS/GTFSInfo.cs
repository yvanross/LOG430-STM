namespace GTFS;

public class GTFSInfo
{
    public string? GetValue(string tag)
    {
        info.TryGetValue(tag, out var value);
        return value;
    }

    internal Dictionary<string, string> info = new Dictionary<string, string>();
}