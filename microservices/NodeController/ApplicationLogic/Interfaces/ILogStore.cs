using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces;

public interface ILogStore
{
    void Log(ISnapshot snapshot);
}