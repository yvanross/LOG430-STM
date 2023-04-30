using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;

namespace ApplicationLogic.Usecases;

public class Ingress
{
    private readonly IIngressClient _ingressClient;
    
    private readonly IHostInfo _hostInfo;

    public Ingress(IHostInfo hostInfo, IIngressClient ingressClient)
    {
        _hostInfo = hostInfo;
        _ingressClient = ingressClient;
    }

    public async Task Register()
    {
        await Try.WithConsequenceAsync(async () =>
            {
                await _ingressClient.Subscribe(
                    _hostInfo.GetTeamName(),
                    _hostInfo.GetUsername(),
                    _hostInfo.GetSecret(),
                    _hostInfo.GetAddress(),
                    _hostInfo.GetPort());

                return Task.CompletedTask;
            },
            retryCount: 2);
    }

    public async Task<string> GetLogStoreAddressAndPort()
    {
        var logStore = await _ingressClient.GetLogStoreAddressAndPort(_hostInfo.GetTeamName());

        return logStore;
    }
}