namespace Infrastructure.FileHandlers.Gtfs;

public interface IDataReader
{
    string GetString(string key);
    object GetObject(string key);
}