using System.Dynamic;
using ApplicationLogic.Extensions;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Ingress.Interfaces;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;

namespace Monitor.Portainer;

public class PortainerClient : IEnvironmentClient
{
    private string _jwt = string.Empty;

    private readonly RestClient _portainerClient;

    private readonly string _username;
    private readonly string _password;

    private string _environmentId = string.Empty;

    private const int _retryCount = 3;

    public PortainerClient(string portainerAddress, string username, string password)
    {
        _portainerClient = new RestClient(portainerAddress);

        _username = username;
        _password = password;
    }

    public async Task<List<ContainerInfo>> GetRunningServices()
    {
        if (string.IsNullOrEmpty(_environmentId))
            _environmentId = await GetEnvironmentId();

        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"api/endpoints/{_environmentId}/docker/containers/json?filters=" + """{"status":["running"]}""", Method.Get);

            request.AddHeader(HeaderNames.Authorization, _jwt);

            var res = await _portainerClient.ExecuteAsync(request);

            dynamic expando = JsonConvert.DeserializeObject<List<ExpandoObject>>(res.Content);

            List<ContainerInfo> microservices = new();

            foreach (var container in expando)
            {
                microservices.Add(new ContainerInfo()
                {
                    Id = container.Id,
                    Name = ((container.Names as List<object>)?.FirstOrDefault()?.ToString())?[1..] ?? string.Empty,
                    ImageName = container.Image,
                    Status = container.Status,
                    Port = container.Ports[1].PublicPort.ToString(),
                });
            }

            return microservices;
        },
        retryCount: _retryCount,
        onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
    }

    public async Task<string> GetContainerLogs(string containerId)
    {
        if (string.IsNullOrEmpty(_environmentId))
            _environmentId = await GetEnvironmentId();

        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"api/endpoints/{_environmentId}/docker/containers/{containerId}/logs?stderr=true&stdout=true", Method.Get);

            request.AddHeader(HeaderNames.Authorization, _jwt);

            var res = await _portainerClient.ExecuteAsync<string>(request);

            return res.Data ?? "container log empty or couldn't fetch it";
        },
        retryCount: _retryCount,
        onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
    }

    public Task IncreaseByOneNumberOfInstances(IContainerConfigName dynamicContainerConfigName, string newContainerName)
    {
        throw new NotImplementedException();
    }

    public async Task IncreaseByOneNumberOfInstances(string containerId, string newContainerName)
    {
        var services = await GetRunningServices();

        var service = services.SingleOrDefault(s => s.Id.Equals(containerId));

        if (service is null) throw new Exception("Service with container Id was not found");

        await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"api/endpoints/{_environmentId}/docker/containers/create", Method.Post);

            request.AddHeader(HeaderNames.Authorization, _jwt);
            
            request.AddJsonBody(new
            {
                Image = service.ImageName,
                Name = newContainerName,
                HostConfig = new
                {
                    RestartPolicy = new
                    {
                        Name = "always"
                    }
                }
            });

            var res = await _portainerClient.ExecuteAsync(request);

            return 0;
        },
        retryCount: _retryCount,
        onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
    }

    public async Task RemoveContainerInstance(string containerId)
    {
        await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"api/endpoints/{_environmentId}/docker/containers/{containerId}", Method.Delete);

            request.AddHeader(HeaderNames.Authorization, _jwt);

            await _portainerClient.ExecuteAsync(request);

            return 0;
        }, retryCount: _retryCount);
    }

    public Task<IContainerConfigName> GetContainerConfig(string containerId)
    {
        throw new NotImplementedException();
    }

    private async Task<string> GetAuthorization()
    {
        return await Try.WithConsequenceAsync<string>(async () =>
        {
            var request = new RestRequest("api/auth", Method.Post);

            request.AddJsonBody(new
            {
                password = _password,
                username = _username
            });

            var res = await _portainerClient.ExecuteAsync(request);

            dynamic expando = JsonConvert.DeserializeObject<ExpandoObject>(res.Content);

            return expando.jwt;
        }, retryCount: _retryCount);
    }

    private async Task<string> GetEnvironmentId()
    {
        return await Try.WithConsequenceAsync<string>(async () =>
        {
            var request = new RestRequest("api/endpoints", Method.Get);

            request.AddHeader(HeaderNames.Authorization, _jwt);

            var res = await _portainerClient.ExecuteAsync(request);

            dynamic expando = JsonConvert.DeserializeObject<List<ExpandoObject>>(res.Content);

            return expando[0].Id;
        }, 
            retryCount: _retryCount, 
            onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
    }
}