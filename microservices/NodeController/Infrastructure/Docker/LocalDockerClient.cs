using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using IO.Swagger.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using ContainerInspectResponse = IO.Swagger.Models.ContainerInspectResponse;
using Try = ApplicationLogic.Extensions.Try;

namespace Infrastructure.Docker;

public class LocalDockerClient : IEnvironmentClient
{
    private readonly RestClient _restClient;
    private readonly ILogger _logger;
    private readonly IHostInfo _hostInfo;
    private const int Timeout = 1000000000;

    public LocalDockerClient(ILogger<LocalDockerClient> logger, IHostInfo hostInfo)
    {
        _logger = logger;
        _hostInfo = hostInfo;
        _restClient = new RestClient("http://host.docker.internal:2375");
    }

    public async Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

        statuses ??= new[] { "running" };

        return await retryPolicy.ExecuteAsync(async () =>
        {
            var restRequest = new RestRequest("containers/json");

            restRequest.AddQueryParameter("filters", JsonConvert.SerializeObject(
                new Dictionary<string, IDictionary<string, bool>>()
                {
                    { "status", statuses.ToDictionary(k => k, v => true) }
                }));

            var res = await _restClient.GetAsync(restRequest);

            var containers = JsonConvert.DeserializeObject<ContainerSummary[]>(res.Content);

            return containers.Select(c => c.Id).ToImmutableList();
        });
    }

    public async Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            var restRequest = new RestRequest($"containers/{containerId}/json");

            var res = await _restClient.GetAsync(restRequest);

            var container = JsonConvert.DeserializeObject<ContainerInspectResponse>(res.Content);

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
                    Id = container.Id,
                    Name = container.Name[1..] ?? string.Empty,
                    ImageName = container.Image,
                    Status = container.State.ToString(),
                    HostPort = ports.hostPort ?? string.Empty,
                    Labels = labelDict,
                    NanoCpus = container.HostConfig.NanoCpus ?? -1L,
                    Memory = container.HostConfig.Memory
                },
                RawConfig: new ContainerRaw()
                {
                    Config = container,
                    ContainerPort = ports.containerPort ?? string.Empty,
                });
        });

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
                container.HostConfig.PortBindings.TryGetValue(portNumber, out var ports);

                if (ports is not null)
                {
                    containerPort = portNumber;
                    portBinding = ports;
                    break;
                }
            }

            var hostPort = portBinding?.FirstOrDefault(p => string.IsNullOrEmpty(p.HostPort) is false)?.HostPort;

            return (hostPort, containerPort);
        }
    }

    public async Task<string?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, string serviceId, string podId)
    {
        string? creationId = null;

        return await Try.WithConsequenceAsync(async () =>
        {
            var env = containerConfig.Config.Config.Env;

            env.RemoveAll(e => e.ToString().StartsWith("ID="));

            env.Add($"ID={serviceId}");

            if (containerConfig.Config.Config.Labels.ContainsKey("POD_ID"))
                containerConfig.Config.Config.Labels["POD_ID"] = podId;
            else
                containerConfig.Config.Config.Labels.Add("POD_ID", podId);

            var exposedPorts = GetPortBindings(containerConfig.ContainerPort);

            containerConfig.Config.HostConfig.RestartPolicy = null;
            containerConfig.Config.HostConfig.AutoRemove = true;
            containerConfig.Config.HostConfig.PortBindings = new PortMap();

            foreach (var ports in exposedPorts)
            {
                containerConfig.Config.HostConfig.PortBindings.Add(ports.Key, ports.Value);
            }

            var restRequest = new RestRequest($"containers/create");

            var payload = new
            {
                HostConfig = containerConfig.Config.HostConfig,
                Env = env,
                Labels = containerConfig.Config.Config.Labels,
                Image = containerConfig.Config.Image,
                ExposedPorts = new Dictionary<string, object>() { { $"{exposedPorts.Keys.First()}", new() } }
            };

            var serializedPayload = JsonConvert.SerializeObject(payload);

            restRequest.RequestFormat = DataFormat.Json;

            restRequest.AddBody(serializedPayload);

            restRequest.AddQueryParameter("name", newContainerName);

            var res = await _restClient.PostAsync(restRequest);

            var containerCreateResponse = JsonConvert.DeserializeObject<ContainerCreateResponse>(res.Content);

            creationId = containerCreateResponse.Id;

            restRequest = new RestRequest($"containers/{creationId}/start");

            res = await _restClient.PostAsync(restRequest);

            return containerCreateResponse.Id;
        },
            onFailure: async (e, _) =>
            {
                _logger.LogCritical(e.ToString());

                if (creationId is not null)
                    await RemoveContainerInstance(creationId, true);
            },
            autoThrow: false,
            retryCount: 2);
    }

    public Task GarbageCollection()
    {
        /*
        await Try.WithConsequenceAsync(async () => await ForceTimeout(_dockerClient.Containers.PruneContainersAsync(
            new ContainersPruneParameters())), autoThrow: false);*/
        return Task.FromResult(0);
    }

    public async Task SetResources(IPodInstance podInstance, long nanoCpus)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

                await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                foreach (var service in podInstance.ServiceInstances)
                {
                    var restRequest = new RestRequest($"containers/{service.ContainerInfo!.Id}/update");

                    var payload = JsonConvert.SerializeObject(new
                    {
                        NanoCpus = nanoCpus,
                    });

                    restRequest.RequestFormat = DataFormat.Json;

                    restRequest.AddBody(payload);

                    var res = await _restClient.PostAsync<ContainerUpdateResponse>(restRequest);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.ToString());
                throw;
            }

            return Task.CompletedTask;
        });
    }

    public async Task RemoveContainerInstance(string containerId, bool quiet = false)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var poolContainers = await GetRunningServices();

            if (poolContainers.Any(c => c.Equals(containerId)))
            {
                var restRequest = new RestRequest($"containers/{containerId}");

               restRequest.AddQueryParameter("v", true);

               restRequest.AddQueryParameter("force", true);

               var res = await _restClient.DeleteAsync(restRequest);
            }

            return Task.CompletedTask;
        }, retryCount: 2, autoThrow: !quiet,
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
}