using Entities.BusinessObjects;
using Ingress.Interfaces;

namespace Entities.DomainInterfaces;

public interface IEnvironmentClient
{
    public Task<List<ContainerInfo>> GetRunningServices();

    public Task<string> GetContainerLogs(string containerId);

    public Task IncreaseByOneNumberOfInstances(IContainerConfigName dynamicContainerConfigName, string newContainerName);
    
    public Task RemoveContainerInstance(string containerId);

    Task<IContainerConfigName> GetContainerConfig(string containerId);
}