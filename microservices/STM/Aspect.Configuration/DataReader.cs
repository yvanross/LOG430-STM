using Application.Common.Extensions;

namespace Aspect.Configuration;

public class DataReader : IDataReader
{
    public string DataFilePath { get; set; } = string.Empty;

    public string GetString(string key)
    {
        var data = File.ReadAllText($"{DataFilePath}/{key}.txt");

        if (data.IsEmpty())
        {
            throw new Exception($"Data not found for key {key}");
        }

        return data;
    }

    public object GetObject(string key)
    {
        var data = File.ReadAllBytes($"{DataFilePath}/{key}.txt");
       
        if (data.IsEmpty())
        {
            throw new Exception($"Data not found for key {key}");
        }
        
        return data;
    }
}