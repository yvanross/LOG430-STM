using System.Collections.Immutable;
using Docker.DotNet.Models;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces;

public interface IEnvironmentClient
{
    public Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default);

    public Task<CreateContainerResponse?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig,
        string newContainerName, string serviceId, string podId);

    public Task RemoveContainerInstance(string containerId, bool quiet = false);

    Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId);

    Task GarbageCollection();

    Task SetResources(IPodInstance podInstance, long nanoCpus, long memory);
}