using Docker.DotNet;
using Entities.BusinessObjects;
using Ingress.Interfaces;

namespace Entities.DomainInterfaces;

public interface IEnvironmentClient
{
    public Task<List<ContainerInfo>> GetRunningServices(string[]? statuses = default);

    public Task<MultiplexedStream> GetContainerLogs(string containerId);

    public Task IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, Guid id);
    
    public Task RemoveContainerInstance(string containerId);

    Task<IContainerConfig> GetContainerConfig(string containerId);
}