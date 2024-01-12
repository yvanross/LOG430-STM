namespace Infrastructure.FileHandlers.StaticGtfs;

public interface IDataReader : IDisposable
{
    string GetString(string key);
  
    StreamReader GetBinary(string key);
}