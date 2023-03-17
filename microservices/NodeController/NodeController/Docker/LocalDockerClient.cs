using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Try = ApplicationLogic.Extensions.Try;

namespace NodeController.Docker;

/// <summary>
/// This is for testing purposes on Docker Desktop
/// </summary>
public class LocalDockerClient : IEnvironmentClient
{
    private static DockerClient _dockerClient = new DockerClientConfiguration(new Uri($"http://{HostInfo.ServiceAddress}:2375")).CreateClient();

    private readonly ILogger _logger;

    public LocalDockerClient(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default)
    {
        statuses ??= new [] { "running" };

        return await Try.WithConsequenceAsync(async () =>
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>()
                {
                    { "status", statuses.ToDictionary(k => k, v => true) }
                }
            });

            return containers.Select(c=>c.ID).ToImmutableList();
        }, retryCount: 5, autoThrow:false);
    }

    public async Task<MultiplexedStream> GetContainerLogs(string containerId)
    {
        return await _dockerClient.Containers.GetContainerLogsAsync(containerId, true, new ContainerLogsParameters());
    }

    public async Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId)
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var container = await _dockerClient.Containers.InspectContainerAsync(containerId);

            var labels = container.Config.Labels
                .Where(kv => Enum.TryParse(typeof(ServiceLabelsEnum), kv.Key, true, out _))
                .ToList()
                .ConvertAll(kv => 
                    new KeyValuePair<ServiceLabelsEnum, string>(
                        (ServiceLabelsEnum)Enum.Parse(typeof(ServiceLabelsEnum), kv.Key, true),
                        kv.Value));

            var labelDict = new ConcurrentDictionary<ServiceLabelsEnum, string>(labels);

            return (
            CuratedInfo: new ContainerInfo()
            {
                Id = container.ID,
                Name = container.Name[1..] ?? string.Empty,
                ImageName = container.Image,
                Status = container.State.Status,
                Port = container.HostConfig.PortBindings["80/tcp"].First(p=>string.IsNullOrEmpty(p.HostPort) is false).HostPort,
                Labels = labelDict
            }, 
            RawConfig: new ContainerConfig()
            {
                Config = container
            });
        },
        retryCount: 5);
    }

    public async Task<CreateContainerResponse?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, string serviceId, string podId)
    {
        CreateContainerResponse? containerResponse = null;

        return await Try.WithConsequenceAsync(async () =>
        {
            var env = (List<string>)containerConfig.Config.Config.Env;

            env.RemoveAll(e => e.ToString().StartsWith("ID="));

            env.Add($"ID={serviceId}");

            var labels = (List<string>)containerConfig.Config.Config.Labels;

            env.RemoveAll(e => e.ToString().StartsWith("POD_ID="));

            env.Add($"POD_ID={podId}");

            var exposedPorts = GetPortBindings();
            
            containerConfig.Config.HostConfig.AutoRemove = true;
            containerConfig.Config.HostConfig.PortBindings = exposedPorts;


            containerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                HostConfig = containerConfig.Config.HostConfig,
                Env = env,
                Image = containerConfig.Config.Image,
                Name = newContainerName,
                ExposedPorts = new Dictionary<string, EmptyStruct>() { {exposedPorts.Keys.First(), new EmptyStruct()} } 
            });

            _ =await _dockerClient.Containers.StartContainerAsync(containerResponse.ID, new ContainerStartParameters());

            return containerResponse;
        }, 
            onFailure: async (_, _) =>
            {
                if(containerResponse is not null)
                    await RemoveContainerInstance(containerResponse.ID).ConfigureAwait(false);
            },
            autoThrow: false,
            retryCount: 5);
    }

    public async Task GarbageCollection()
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var containersToRemove = await GetRunningServices(new[] { "exited", "dead" });

            await Parallel
                .ForEachAsync(containersToRemove, async (toKill, _) => await RemoveContainerInstance(toKill))
                .ConfigureAwait(false);

            return Task.CompletedTask;
        });
    }

    public async Task RemoveContainerInstance(string containerId)
    {
        try
        {
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true });
        }
        catch
        {
            // ignored
        }
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    private IDictionary<string, IList<PortBinding>> GetPortBindings()
    {
        return new Dictionary<string, IList<PortBinding>>
        {
            { "80/tcp", new List<PortBinding>
                {
                    new()
                    {
                        HostPort = Random.Shared.Next(40000, 50000).ToString()
                    }
                }
            }
        };
    }
}