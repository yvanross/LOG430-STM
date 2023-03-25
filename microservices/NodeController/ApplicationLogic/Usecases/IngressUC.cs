using System.Globalization;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;

namespace ApplicationLogic.Usecases;

public class IngressUC
{
    private readonly IIngressClient _ingressClient;
    
    private readonly IHostInfo _hostInfo;

    public IngressUC(IHostInfo hostInfo, IIngressClient ingressClient)
    {
        _hostInfo = hostInfo;
        _ingressClient = ingressClient;
    }

    public async Task Register()
    {
        await Try.WithConsequenceAsync(async () =>
            {
                await _ingressClient.Subscribe(_hostInfo.GetTeamName(), _hostInfo.GetAddress(), _hostInfo.GetPort());

                return Task.CompletedTask;
            },
            retryCount: 100);
    }

    public async Task<string> GetLogStoreAddressAndPort()
    {
        var logStore = await _ingressClient.GetLogStoreAddressAndPort(_hostInfo.GetTeamName());

        return logStore;
    }
}