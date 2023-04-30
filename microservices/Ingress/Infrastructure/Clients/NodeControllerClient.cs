using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.Clients;

public class NodeControllerClient : INodeControllerClient
{
    private readonly ILogger _logger;

    public NodeControllerClient(ILogger logger)
    {
        _logger = logger;
    }

    public async Task BeginExperiment(string hostAddressAndPort, string experimentDto)
    {
        var client = new RestClient(hostAddressAndPort);

        var request = new RestRequest("Experiment/Begin");

        request.AddStringBody(experimentDto, DataFormat.Json);

        var res = await client.ExecutePostAsync(request);

        res.ThrowIfError();

        _logger.LogInformation($"experiment sent to {hostAddressAndPort}");
    }

}