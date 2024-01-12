using System.IO.Compression;
using System.Text;
using Infrastructure.FileHandlers.StaticGtfs;

namespace Configuration;

public class DataReader : IDataReader
{
    private FileStream? _file;

    private GZipStream ? _gzipStream;

    private StreamReader? _streamReader;

    private readonly string _basePath = Environment.GetEnvironmentVariable("BASE_PATH") ?? "Data/";

    public string GetString(string key)
    {
        var data = File.ReadAllText($"{_basePath}{key}.txt");

        if (string.IsNullOrWhiteSpace(data)) throw new Exception($"Data not found for key {key}");
       
        return data;
    }

    public StreamReader GetBinary(string key)
    {
        _file?.Dispose();
        _gzipStream?.Dispose();
        _streamReader?.Dispose();

        _file = new FileStream($"{_basePath}{key}.bin", FileMode.Open, FileAccess.Read);

        _gzipStream = new GZipStream(_file, CompressionMode.Decompress);

        _streamReader = new StreamReader(_gzipStream, Encoding.UTF8);

        if (_streamReader is null) throw new Exception($"Data not found for key {key}");

        return _streamReader;
    }

    public void Dispose()
    {
        _file?.Dispose();
        _gzipStream?.Dispose();
        _streamReader?.Dispose();
    }
}