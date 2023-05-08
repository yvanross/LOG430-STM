using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using MqContracts;

namespace ApplicationLogic.Usecases;

public class Ingress
{
    private readonly IIngressClient _ingressClient;
    private readonly IHeartbeatService _heartbeatService;
    private readonly IHostInfo _hostInfo;


    public Ingress(IHostInfo hostInfo, IIngressClient ingressClient, IHeartbeatService heartbeatService)
    {
        _hostInfo = hostInfo;
        _ingressClient = ingressClient;
        _heartbeatService = heartbeatService;
    }

    public async Task Register()
    {
        await _ingressClient.Subscribe(
            _hostInfo.GetGroup(),
            _hostInfo.GetTeamName(),
            _hostInfo.GetUsername(),
            _hostInfo.GetSecret(),
            _hostInfo.GetAddress(),
            _hostInfo.GetPort());
    }

    public async Task<string> GetLogStoreAddressAndPort()
    {
        var logStore = await _ingressClient.GetLogStoreAddressAndPort(_hostInfo.GetTeamName());

        return logStore;
    }

    public async Task HeartBeat()
    {
        await _heartbeatService.Produce(new HeartBeatDto()
        {
            Source = _hostInfo.GetUsername(),
            Version = _hostInfo.GetVersion(),
            Secure = _hostInfo.GetSecure(),
            Dirty =  _hostInfo.GetIsDirty()
        });
    }
}