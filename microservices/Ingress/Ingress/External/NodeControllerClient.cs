using Ingress.Dto;
using NodeController.External.Docker;
using RestSharp;

namespace Ingress.External;

public class NodeControllerClient
{
    private readonly RestClient _client;

    private readonly ILogger _logger;

    public NodeControllerClient(string hostAddressAndPort, ILogger logger)
    {
        _logger = logger;
        _client = new RestClient(hostAddressAndPort);
    }

    public async Task BeginExperiment(string experimentDto)
    {
        var request = new RestRequest("Experiment/Begin");

        request.AddStringBody(experimentDto, DataFormat.Json);

        var res = await _client.ExecutePostAsync(request);

        res.ThrowIfError();
    }

}