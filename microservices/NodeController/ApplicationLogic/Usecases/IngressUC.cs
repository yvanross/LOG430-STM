using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases;

public class IngressUC
{
    private readonly IIngressClient _ingressClient;
    
    private readonly IHostInfo _hostInfo;

    private readonly ILogger _logger;

    public IngressUC(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Register(string teamName)
    {
        await Try.WithConsequenceAsync(async () =>
            {
                _logger?.LogInformation($"Attempting to subscribe to IngressController");

                await _ingressClient.Subscribe(teamName, _hostInfo.GetAddress(), _hostInfo.GetPort());

                return Task.CompletedTask;
            },
            retryCount: 100);
    }

    public async Task<string> GetLogStore()
    {
        var logStore = await _ingressClient.GetLogStore();

        return logStore;
    }
}