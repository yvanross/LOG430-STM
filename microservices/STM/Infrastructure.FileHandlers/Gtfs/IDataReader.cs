namespace Aspect.Configuration;

public interface IDataReader
{
    string GetString(string key);
    object GetObject(string key);
}