using System.Collections;
using System.Dynamic;
using Ambassador.BusinessObjects;
using ApplicationLogic.Extensions;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Microsoft.Net.Http.Headers;
using Monitor.Portainer;
using Newtonsoft.Json;
using RestSharp;

namespace Monitor.Docker;

/// <summary>
/// This is for testing purposes on Docker Desktop
/// </summary>
public class LocalDockerClient : IEnvironmentClient
{
    private readonly RestClient _dockerClient = new ($"http://{EnvironmentVariables.ServiceAddress}:2375");

    public async Task<List<Microservice>> GetRunningServices()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest("containers/json", Method.Get);

            var res = await _dockerClient.ExecuteAsync(request);

            dynamic expando = JsonConvert.DeserializeObject<List<ExpandoObject>>(res.Content);

            List<Microservice> microservices = new();

            foreach (var container in expando)
            {
                dynamic? port = (container.Ports as List<object>)?.FirstOrDefault(x => ((IDictionary<string, object>)x).ContainsKey("PublicPort"));

                string? publicPort = port?.PublicPort.ToString();

                microservices.Add(new Microservice()
                {
                    Id = container.Id,
                    Name = ((container.Names as List<object>)?.FirstOrDefault()?.ToString())?[1..] ?? string.Empty,
                    ImageName = container.Image,
                    Status = container.Status,
                    Port = publicPort!
                });
            }

            return microservices;
        }, retryCount: 2);
    }

    public Task<string> GetContainerLogs(string containerId)
    {
        throw new NotImplementedException();
    }

    public async Task IncreaseByOneNumberOfInstances(string containerId, string newContainerName)
    {
        var services = await GetRunningServices();

        var service = services.SingleOrDefault(s => s.Id.Equals(containerId));

        if (service is null) throw new Exception("Service with container Id was not found");

        var containerData = await Try.WithConsequenceAsync<(dynamic Config, dynamic HostConfig)>(async () =>
            {
                var request = new RestRequest($"containers/{service.Id}/json", Method.Get);

                var res = await _dockerClient.ExecuteAsync(request);

                dynamic expando = JsonConvert.DeserializeObject<ExpandoObject>(res.Content);

                return (expando.Config, expando.HostConfig);
            },
            retryCount: 2);

        var newId = await Try.WithConsequenceAsync<string>(async () =>
            {
                var request = new RestRequest($"containers/create?name={newContainerName}", Method.Post);

                var body = containerData.Config;

                body.Name = newContainerName;
                body.HostConfig = containerData.HostConfig;

                request.AddJsonBody((object)body);

                var res = await _dockerClient.ExecuteAsync(request);

                dynamic expando = JsonConvert.DeserializeObject<ExpandoObject>(res.Content);

                return expando.Id;
            },
            retryCount: 2);

        await Try.WithConsequenceAsync(async () =>
            {
                var request = new RestRequest($"containers/{newId}/start", Method.Post);

                var res = await _dockerClient.ExecuteAsync(request);

                return 0;
            });
    }

    public async Task RemoveContainerInstance(string containerId)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"containers/{containerId}", Method.Delete);

            await _dockerClient.ExecuteAsync(request);

            return 0;
        }, retryCount: 2);
    }
}