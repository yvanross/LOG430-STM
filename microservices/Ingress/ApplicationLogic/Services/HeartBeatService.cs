using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;

namespace ApplicationLogic.Services;

public class HeartBeatService
{
    private readonly IRepositoryRead _readModel;
    
    private readonly IRepositoryWrite _writeModel;

    private static readonly PeriodicTimer PeriodicTimer = new(TimeSpan.FromMilliseconds(0.5));
    
    private static Task? _clock;

    private readonly ConcurrentQueue<Guid> _potentiallyDeadServices = new ();

    public HeartBeatService(IRepositoryRead readModel, IRepositoryWrite writeModel)
    {
        _readModel = readModel;
        _writeModel = writeModel;

        if (_clock is null)
            _clock = BeginSendingHeartbeats();
    }

    public void Acknowledge(Guid id)
    {
        var route = _readModel.ReadServiceById(id);

        if (route is not null)
        {
            route.LastHeartbeat = DateTime.UtcNow;

            _writeModel.Write(route);
        }
    }

    internal async Task BeginSendingHeartbeats()
    {
        while (await PeriodicTimer.WaitForNextTickAsync())
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var routes = _readModel.GetAllServices();

                while (_potentiallyDeadServices.IsEmpty is false)
                {
                    _potentiallyDeadServices.TryDequeue(out var idToAudit);

                    var serviceToAudit = _readModel.ReadServiceById(idToAudit);

                    if(serviceToAudit is not null && serviceToAudit.LastHeartbeat >= DateTime.UtcNow.AddSeconds(-1))
                        

                }

                 = routes.Where(r => r.LastHeartbeat <= DateTime.UtcNow.AddSeconds(-0.5));

                

                return Task.FromResult(0);
            }, retryCount: int.MaxValue);
        }
    }
}