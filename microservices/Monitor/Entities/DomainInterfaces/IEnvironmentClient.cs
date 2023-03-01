using Entities.BusinessObjects;

namespace Entities.DomainInterfaces;

public interface IEnvironmentClient
{
    public Task<List<Microservice>> GetRunningContainersIds();

    public Task<string> GetContainerLogs(string containerId);

    //todo error from the school portainer for some reason
    //public Task<string> GetContainerCpuUsage();
}