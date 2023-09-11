using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using IO.Swagger.Model;
using IO.Swagger.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using ContainerInspectResponse = IO.Swagger.Models.ContainerInspectResponse;
using Port = Entities.BusinessObjects.Live.Port;
using Try = Entities.Extensions.Try;

namespace Infrastructure.Docker;

public class DockerdClient : IEnvironmentClient
{
    private readonly RestClient _restClient;
    private readonly ILogger _logger;
    private readonly IHostInfo _hostInfo;

    public DockerdClient(ILogger<DockerdClient> logger, IHostInfo hostInfo)
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
            try
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting running services");

                throw;
            }
            
        });
    }

    public async Task<ImmutableList<string>?> GetVolumes()
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            var restRequest = new RestRequest("volumes");

            restRequest.AddQueryParameter("filters", JsonConvert.SerializeObject(
                new Dictionary<string, string>()
                {
                    { "dangling", "false" }
                }));

            var res = await _restClient.GetAsync(restRequest);

            var volumesRes = JsonConvert.DeserializeObject<VolumeListResponse>(res.Content);

            return volumesRes.Volumes.ConvertAll(v=>v.Name).ToImmutableList();
        });
    }

    public async Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
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

                var ports = GetPortsInfo(container);

                return (
                    CuratedInfo: new ContainerInfo()
                    {
                        Id = container.Id,
                        Name = container.Name[1..] ?? string.Empty,
                        ImageName = container.Image,
                        Status = container.State.ToString(),
                        PortsInfo = ports,
                        Labels = labelDict,
                        NanoCpus = container.HostConfig.NanoCpus ?? -1L,
                        Memory = container.HostConfig.Memory
                    },
                    RawConfig: new ContainerRaw()
                    {
                        Config = container,
                        PortsInfo = ports,
                    });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting container info");

                throw;
            }
        });

        PortsInfo GetPortsInfo(ContainerInspectResponse container)
        {
            var routingContainerPorts = new List<string>(
                _hostInfo.GetCustomContainerPorts()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Where(s => string.IsNullOrWhiteSpace(s) is false))
            {
                "80"
            };

            var ports = container.HostConfig.PortBindings.ToList().ConvertAll(GetPort);
            
            var routingPortNumber = ports.FirstOrDefault(p => routingContainerPorts.Contains(p?.containerPort!))?.hostPort ?? 0;

            return new PortsInfo()
            {
                RoutingPortNumber = routingPortNumber,
                Ports = ports.ConvertAll(p => new Port(p.Value.hostPort, int.Parse(p.Value.containerPort), p.Value.TransportProtocol))
            };

            (int hostPort, string containerPort, string TransportProtocol)? GetPort(KeyValuePair<string, IList<PortBinding>> input)
            {
                var hostPort = input.Value.FirstOrDefault(port => string.IsNullOrWhiteSpace(port.HostPort) is false)?.HostPort ?? "0";

                var split = input.Key.Split('/');

                return (int.Parse(hostPort), split[0], split[1]);
            }
        }
    }

    public async Task<string?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName, IServiceInstance serviceInstance, IPodType podType)
    {
        string? creationId = null;

        return await Try.WithConsequenceAsync(async () =>
        {
            var env = containerConfig.Config.Config.Env;

            env.RemoveAll(e => e.ToString().StartsWith("ID="));

            env.Add($"ID={serviceInstance.Id}");

            containerConfig.Config.Config.Labels["POD_ID"] = serviceInstance.PodId;

            containerConfig.Config.Config.Labels.TryAdd("ARTIFACT_NAME", serviceInstance.Type);
            containerConfig.Config.Config.Labels.TryAdd("POD_NAME", podType.Type);

            var exposedPorts = GetPortBindings(new List<string>(containerConfig.PortsInfo.Ports.ConvertAll(p=>p.ToString())));

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
                Hostname = newContainerName,
                HostConfig = containerConfig.Config.HostConfig,
                Env = env,
                Labels = containerConfig.Config.Config.Labels,
                Image = containerConfig.Config.Image,
                ExposedPorts = new Dictionary<string, object>() { { $"{exposedPorts.Keys.First()}", new() } },
                Volumes = podType.ShareVolumes ? containerConfig.Config.Config.Volumes : null,
                Cmd = containerConfig.Config.Config.Cmd
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
            onFailure: async (e, i) =>
            {
                _logger.LogError(e, $"Error while creating container, compensating transaction. Will retry immediately: {i< 2}");

                if (creationId is not null)
                    await RemoveContainerInstance(creationId);
            },
            autoThrow: true,
            retryCount: 2);
    }

    public Task GarbageCollection()
    {//not using this for now
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

    public async Task RemoveContainerInstance(string containerId, bool soft = false, bool quiet = false)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var poolContainers = await GetRunningServices(new [] { "running", "paused", "exited", "dead", "created", "restarting" });

            if (poolContainers.Any(c => c.Equals(containerId)))
            {
                if (soft)
                {
                    var softClose = new RestRequest($"containers/{containerId}/kill");

                    softClose.AddQueryParameter("signal", "SIGTERM");

                    var _ = await _restClient.PostAsync(softClose);
                }

                var restRequest = new RestRequest($"containers/{containerId}");

               restRequest.AddQueryParameter("v", true);

               restRequest.AddQueryParameter("force", true);

               var res = await _restClient.DeleteAsync(restRequest);

               res.ThrowIfError();
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

    public async Task RemoveVolume(string name)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, (_) => TimeSpan.FromMilliseconds(100));

        await retryPolicy.ExecuteAsync(async () =>
        {
            var restRequest = new RestRequest($"volumes/{name}");

            restRequest.AddQueryParameter("force", true);

            var res = await _restClient.DeleteAsync(restRequest);
        });
    }

    private IDictionary<string, IList<PortBinding>> GetPortBindings(List<string> containerPort)
    {
        var dict = new Dictionary<string, IList<PortBinding>>();

        foreach (var port in containerPort)
        {
            dict.Add(port, new List<PortBinding>
                {
                    new()
                    {
                        HostPort = Random.Shared.Next(40000, 50000).ToString()
                    }
                });
        }

        return dict;
    }
}