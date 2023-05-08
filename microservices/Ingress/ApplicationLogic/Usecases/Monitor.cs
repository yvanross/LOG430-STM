using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;
using System.Net.Sockets;
using ApplicationLogic.Interfaces.Dao;
using Microsoft.Extensions.Logging;
using MqContracts;
using Newtonsoft.Json;

namespace ApplicationLogic.Usecases
{
    public class Monitor
    {
        private readonly IRepositoryRead _readModel;
        private readonly IRepositoryWrite _writeModel;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authService;
        private readonly IAckErrorEmitter<AckErrorDto> _ackErrorEmitter;

        public Monitor(IRepositoryRead readModel, IRepositoryWrite writeModel, ILogger<Monitor> logger, IAuthorizationService authService, IAckErrorEmitter<AckErrorDto> ackErrorEmitter)
        {
            _readModel = readModel;
            _writeModel = writeModel;
            _logger = logger;
            _authService = authService;
            _ackErrorEmitter = ackErrorEmitter;
        }

        public async Task BeginProcessingHeartbeats()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var routes = _readModel.GetAllNodes();

                //if empty or null
                if ((routes?.Any() ?? true) is false) return Task.FromResult(Task.CompletedTask);

                var unknownStateRoutes = routes!.Where(node =>
                {
                    node.ServiceStatus?.EvaluateState(node);

                    return node.ServiceStatus is UnknownState;
                }).ToList();

                foreach (var node in unknownStateRoutes)
                {
                    _logger.LogCritical($"Lost connection to {node.Name}, last known state {node.LastSuccessfulPing}");

                    await _authService.Remove(node.Name);

                    _writeModel.RemoveNode(node);
                }

                return Task.FromResult(Task.CompletedTask);
            }, retryCount: int.MaxValue);
        }

        public void TryAcknowledge(string name, string version, bool secure, bool dirty)
        {
            var route = _readModel.ReadNodeById(name);

            if (route is not null)
            {
                route.LastSuccessfulPing = DateTime.UtcNow;
                route.Version = version;
                route.Secure = secure;
                route.Dirty = dirty;
            }
            else
            {
                _ackErrorEmitter.Produce(name, new AckErrorDto());
            }
        }
    }
}
