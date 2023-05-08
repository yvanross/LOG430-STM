using System.Collections.Concurrent;

namespace Entities.DomainInterfaces;

public interface ISystemStateReadService
{
    Task<ConcurrentDictionary<string, object?>> GetStates(IEnumerable<string> names, string group);
}