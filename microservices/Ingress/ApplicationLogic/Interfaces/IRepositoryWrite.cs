using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryWrite
{
    void AddOrUpdateNode(INode node);
    
    void RemoveNode(INode node);
}