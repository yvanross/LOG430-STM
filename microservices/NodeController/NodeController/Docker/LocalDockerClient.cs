using System.Runtime.CompilerServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Try = ApplicationLogic.Extensions.Try;

namespace NodeController.Docker;

/// <summary>
/// This is for testing purposes on Docker Desktop
/// </summary>
public class LocalDockerClient : IEnvironmentClient
{
    private readonly DockerClient _dockerClient = new DockerClientConfiguration(new Uri("http://host.docker.internal:2375")).CreateClient();

    private readonly ILogger _logger;

    public LocalDockerClient(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<List<ContainerInfo>> GetRunningServices(string[]? statuses = default)
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

            List<ContainerInfo> microservices = new();

            foreach (var container in containers)
            {
                var publicPort = container.Ports.First(p=>p.PrivatePort.Equals(80)).PublicPort.ToString();

                microservices.Add(new ContainerInfo()
                {
                    Id = container.ID,
                    Name = container.Names?.FirstOrDefault()?[1..] ?? string.Empty,
                    ImageName = container.Image,
                    Status = container.Status,
                    Port = publicPort
                });
            }

            return microservices;
        }, retryCount: 5, autoThrow:false);
    }

    public async Task<MultiplexedStream> GetContainerLogs(string containerId)
    {
        return await _dockerClient.Containers.GetContainerLogsAsync(containerId, true, new ContainerLogsParameters());
    }

    public async Task<IContainerConfig> GetContainerConfig(string containerId)
    {
        var containerData = await Try.WithConsequenceAsync(async () =>
        {
           var container = await _dockerClient.Containers.InspectContainerAsync(containerId);

           return container;
        },
        retryCount: 5);

        return new ContainerConfig()
        {
            Config = containerData
        };
    }

    public async Task IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, Guid id)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var env = (List<string>)containerConfig.Config.Config.Env;

            env.RemoveAll(e => e.ToString().StartsWith("ID="));

            env.Add($"ID={id}");

            var exposedPorts = GetPortBindings();
            
            containerConfig.Config.HostConfig.AutoRemove = true;
            containerConfig.Config.HostConfig.PortBindings = exposedPorts;


            var containerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                HostConfig = containerConfig.Config.HostConfig,
                Env = env,
                Image = containerConfig.Config.Image,
                Name = newContainerName,
                ExposedPorts = new Dictionary<string, EmptyStruct>() { {exposedPorts.Keys.First(), new EmptyStruct()} } 
            });

            _ =await _dockerClient.Containers.StartContainerAsync(containerResponse.ID, new ContainerStartParameters());

            return Task.CompletedTask;
        },
        retryCount: 5, onFailure: async (_, _) => await GarbageCollection());

        await GarbageCollection();

        async Task GarbageCollection()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var containersToRemove = await GetRunningServices(new[] { "exited", "dead" });

                await Parallel
                    .ForEachAsync(containersToRemove, async (toKill, _) => await RemoveContainerInstance(toKill.Id))
                    .ConfigureAwait(false);

                return Task.CompletedTask;
            },
            retryCount: 2, onFailure: (e, _) =>
            {
                _logger.LogInformation(e.Message + "\n" + e.InnerException);
                return Task.CompletedTask;
            });
        }
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