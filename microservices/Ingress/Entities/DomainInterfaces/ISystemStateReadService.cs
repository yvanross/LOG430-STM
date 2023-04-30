namespace Entities.DomainInterfaces;

public interface ISystemStateReadService
{
    Task<IEnumerable<object>> ReadLogs(IEnumerable<string> names);
}