using System.Resources;
using Infrastructure.FileHandlers.Gtfs;

namespace Aspect.Configuration;

public class DataReader : IDataReader
{
    private readonly ResourceManager _manager;

    public DataReader(ResourceManager manager)
    {
        _manager = manager;
    }

    public string GetString(string key)
    {
        var data = _manager.GetString(key);

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new Exception($"Data not found for key {key}");
        }

        return data;
    }

    public object GetObject(string key)
    {
        var data = _manager.GetObject(key);

        if (data is null)
        {
            throw new Exception($"Data not found for key {key}");
        }
        
        return data;
    }
}