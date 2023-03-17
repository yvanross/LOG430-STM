using System.Collections.Immutable;
using Docker.DotNet;
using Docker.DotNet.Models;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface IEnvironmentClient
{
    public Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default);

    public Task<CreateContainerResponse?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig,
        string newContainerName, string serviceId, string podId);

    public Task RemoveContainerInstance(string containerId);

    Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId);

    Task GarbageCollection();
}