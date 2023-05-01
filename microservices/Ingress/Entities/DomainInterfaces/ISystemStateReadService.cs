using System.Collections.Concurrent;

namespace Entities.DomainInterfaces;

public interface ISystemStateReadService
{
    Task<ConcurrentDictionary<string, IEnumerable<object?>>> ReadLogs(IEnumerable<string> names, string group);
}