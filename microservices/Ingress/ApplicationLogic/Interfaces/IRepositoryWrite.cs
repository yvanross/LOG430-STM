using Entities.DomainInterfaces;
using Ingress.Interfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryWrite
{
    void WriteService(IService service);
    
    void RemoveService(IService service);

    void UpdateContainerModel(IService service, IContainerConfigName containerConfigName);

}