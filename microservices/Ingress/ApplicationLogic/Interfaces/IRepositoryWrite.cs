using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryWrite
{
    void Write(IRoute route);
}