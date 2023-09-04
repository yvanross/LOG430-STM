namespace Infrastructure.FileHandlers.Gtfs;

public class GtfsInfo : IDisposable
{
    public readonly Dictionary<string, string> Info = new();

    public void Dispose()
    {
        Info.Clear();
    }

    public string GetValue(string tag)
    {
        Info.TryGetValue(tag, out var value);

        if (value is null)
            throw new ArgumentException($"Tag {tag} not found in GTFS info");

        return value;
    }
}