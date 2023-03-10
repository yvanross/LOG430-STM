using Entities.BusinessObjects;

namespace Entities.DomainInterfaces;

public interface IEnvironmentClient
{
    public Task<List<ContainerInfo>> GetRunningServices();

    public Task<string> GetContainerLogs(string containerId);

    public Task IncreaseByOneNumberOfInstances(string containerId, string newContainerName);
    
    public Task RemoveContainerInstance(string containerId);

    //todo error from the school portainer for some reason
    //public Task<string> GetContainerCpuUsage();
}