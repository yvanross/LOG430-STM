using System.Dynamic;
using ApplicationLogic.Extensions;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;

namespace Monitor.Docker;

/// <summary>
/// This is for testing purposes on Docker Desktop
/// </summary>
public class LocalDockerClient : IEnvironmentClient
{
    private readonly RestClient _dockerClient = new ("http://localhost:2375/");

    public async Task<List<Microservice>> GetRunningContainersIds()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest("containers/json", Method.Get);

            var res = await _dockerClient.ExecuteAsync(request);

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
        }, retryCount: 2);
    }

    public Task<string> GetContainerLogs(string containerId)
    {
        throw new NotImplementedException();
    }
}