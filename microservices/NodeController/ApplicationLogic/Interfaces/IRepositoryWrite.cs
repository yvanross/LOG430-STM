using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryWrite
{
    void WriteService(IServiceInstance serviceInstance);
    
    void RemoveService(IServiceInstance serviceInstance);

    void UpdateServiceType(IServiceInstance serviceInstance, IServiceType containerConfigName);

}