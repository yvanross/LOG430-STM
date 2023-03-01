using System.Dynamic;
using ApplicationLogic.Extensions;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
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

    public PortainerClient(string portainerAddress, string username, string password)
    {
        _portainerClient = new RestClient(portainerAddress);

        _username = username;
        _password = password;
    }

    public async Task<List<Microservice>> GetRunningContainersIds()
    {
        if (string.IsNullOrEmpty(_environmentId))
            _environmentId = await GetEnvironmentId();

        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"api/endpoints/{_environmentId}/docker/containers/json?filters=" + """{"status":["running"]}""", Method.Get);

            request.AddHeader(HeaderNames.Authorization, _jwt);

            var res = await _portainerClient.ExecuteAsync(request);

            dynamic expando = JsonConvert.DeserializeObject<List<ExpandoObject>>(res.Content);

            List<Microservice> microservices = new();

            foreach (var container in expando)
            {
                microservices.Add(new Microservice()
                {
                    Id = container.Id,
                    Name = (container.Names as ICollection<string>)?.FirstOrDefault() ?? string.Empty,
                    ImageName = container.Image,
                    Status = container.Status
                });
            }

            return microservices;
        },
        retryCount: 2,
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
        retryCount: 2,
        onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
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
        }, retryCount: 2);
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
            retryCount: 2, 
            onFailure: async (_, _) => _jwt = $"bearer {await GetAuthorization()}");
    }
}