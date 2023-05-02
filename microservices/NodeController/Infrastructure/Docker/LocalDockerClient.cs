using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using ApplicationLogic.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Microsoft.Extensions.Logging;
using Try = ApplicationLogic.Extensions.Try;

namespace Infrastructure.Docker;

/// <summary>
/// This is for testing purposes on Docker Desktop
/// </summary>
public class LocalDockerClient : IEnvironmentClient
{
    private static DockerClient? _dockerClient;
    private readonly ILogger _logger;
    private readonly IHostInfo _hostInfo;
    private const int timeout = 1000;


    public LocalDockerClient(ILogger<LocalDockerClient> logger, IHostInfo hostInfo)
    {
        _logger = logger;
        _hostInfo = hostInfo;
        _dockerClient ??= new DockerClientConfiguration(
                new Uri("http://host.docker.internal:2375"), defaultTimeout:TimeSpan.FromSeconds(5))
            .CreateClient();
    }

    public async Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default)
    {
        statuses ??= new[] { "running" };

        return await Try.WithConsequenceAsync(async () =>
        {
            var containers = await ForceTimeout(_dockerClient!.Containers.ListContainersAsync(new ContainersListParameters()
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>()
                {
                    { "status", statuses.ToDictionary(k => k, v => true) }
                }
            }));

            return containers.Select(c => c.ID).ToImmutableList();
        }, retryCount: 2, autoThrow: false);
    }

    public async Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId)
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var container = await ForceTimeout(_dockerClient!.Containers.InspectContainerAsync(containerId));

            var labels = container.Config.Labels
                .Where(kv => Enum.TryParse(typeof(ServiceLabelsEnum), kv.Key, true, out _))
                .ToList()
                .ConvertAll(kv =>
                    new KeyValuePair<ServiceLabelsEnum, string>(
                        (ServiceLabelsEnum)Enum.Parse(typeof(ServiceLabelsEnum), kv.Key, true),
                        kv.Value));

            var labelDict = new ConcurrentDictionary<ServiceLabelsEnum, string>(labels);

            var ports = GetPortForDefaultProtocols(container);

            return (
            CuratedInfo: new ContainerInfo()
            {
                Id = container.ID,
                Name = container.Name[1..] ?? string.Empty,
                ImageName = container.Image,
                Status = container.State.Status,
                HostPort = ports.hostPort ?? string.Empty,
                Labels = labelDict,
                NanoCpus = container.HostConfig.NanoCPUs,
                Memory = container.HostConfig.Memory,
            },
            RawConfig: new ContainerConfig()
            {
                Config = container,
                ContainerPort = ports.containerPort ?? string.Empty,
            });
        },
        retryCount: 2, autoThrow: false);

        (string? hostPort, string? containerPort) GetPortForDefaultProtocols(ContainerInspectResponse container)
        {
            var containerPorts = new List<string>() { "80/tcp" };

            _hostInfo.GetCustomContainerPorts()
                .Split(',', StringSplitOptions.TrimEntries)
                .Where(s=>string.IsNullOrWhiteSpace(s) is false)
                .ToList()
                .ForEach(containerPort => containerPorts.Add($"{containerPort}/tcp"));

            IList<PortBinding>? portBinding = null;

            string? containerPort = null;

            foreach (var portNumber in containerPorts)
            {
                portBinding = container.HostConfig.PortBindings.FirstOrDefault(kv => kv.Key.Equals(portNumber)).Value;

                if (portBinding is not null)
                {
                    containerPort = portNumber;
                    break;
                }
            }

            var hostPort = portBinding?.FirstOrDefault(p => string.IsNullOrEmpty(p.HostPort) is false)?.HostPort;
            
            return (hostPort, containerPort);
        }
    }

    public async Task<CreateContainerResponse?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, string serviceId, string podId)
    {
        CreateContainerResponse? containerResponse = null;

        return await Try.WithConsequenceAsync(async () =>
        {
            var env = (List<string>)containerConfig.Config.Config.Env;

            env.RemoveAll(e => e.ToString().StartsWith("ID="));

            env.Add($"ID={serviceId}");

            if (containerConfig.Config.Config.Labels.ContainsKey("POD_ID"))
                containerConfig.Config.Config.Labels["POD_ID"] = podId;
            else
                containerConfig.Config.Config.Labels.Add("POD_ID", podId);

            var exposedPorts = GetPortBindings(containerConfig.ContainerPort);

            containerConfig.Config.HostConfig.RestartPolicy = null;
            containerConfig.Config.HostConfig.AutoRemove = true;
            containerConfig.Config.HostConfig.PortBindings = exposedPorts;


            containerResponse = await ForceTimeout(_dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                HostConfig = containerConfig.Config.HostConfig,
                Env = env,
                Labels = containerConfig.Config.Config.Labels,
                Image = containerConfig.Config.Image,
                Name = newContainerName,
                ExposedPorts = new Dictionary<string, EmptyStruct>() { { exposedPorts.Keys.First(), new EmptyStruct() } }
            }));

            _ = await ForceTimeout(_dockerClient.Containers.StartContainerAsync(containerResponse.ID, new ContainerStartParameters()));

            return containerResponse;
        },
            onFailure: async (e, _) =>
            {
                _logger.LogCritical(e.Message);
                _logger.LogDebug(e.StackTrace);

                if (containerResponse is not null)
                    await RemoveContainerInstance(containerResponse.ID, true);
            },
            autoThrow: false,
            retryCount: 2);
    }

    public async Task GarbageCollection()
    {
        await Try.WithConsequenceAsync(async () => await ForceTimeout(_dockerClient.Containers.PruneContainersAsync(
            new ContainersPruneParameters())), autoThrow: false);
    }

    public async Task SetResources(IPodInstance podInstance, long nanoCpus, long memory)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            foreach (var service in podInstance.ServiceInstances)
            {
                await ForceTimeout(_dockerClient!.Containers.UpdateContainerAsync(service.ContainerInfo!.Id,
                    new ContainerUpdateParameters()
                    {
                        NanoCPUs = nanoCpus,
                        Memory = memory,
                    }));
            }

            return Task.CompletedTask;
        }, retryCount: 2, autoThrow: false);
    }

    public async Task RemoveContainerInstance(string containerId, bool quiet = false)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var poolContainers = await ForceTimeout(_dockerClient!.Containers.ListContainersAsync(new ContainersListParameters()));

            if(poolContainers.Any(c=>c.ID.Equals(containerId)))
                await ForceTimeout(_dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true }));

            return Task.CompletedTask;
        }, retryCount: 2, autoThrow: false,
            onFailure: (e, _) =>
            {
                if (quiet is false)
                {
                    _logger?.LogCritical(e.Message);
                    _logger?.LogDebug(e.StackTrace);
                }
                
                return Task.CompletedTask;
            });
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    private IDictionary<string, IList<PortBinding>> GetPortBindings(string containerPort)
    {
        return new Dictionary<string, IList<PortBinding>>
        {
            { containerPort, new List<PortBinding>
                {
                    new()
                    {
                        HostPort = Random.Shared.Next(40000, 50000).ToString()
                    }
                }
            }
        };
    }

    private static async Task ForceTimeout(Task t)
    {
        var token = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(t, token);

        if (completedTask == token)
        {
            throw new TimeoutException("Docker Daemon jammed again, forcing timeout");
        }
    }

    private static async Task<T> ForceTimeout<T>(Task<T> t)
    {
        var token = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(t, token);

        if (completedTask == token)
        {
            throw new TimeoutException("Docker Daemon jammed again, forcing timeout");
        }

        return t.Result;
    }
}