using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Docker.DotNet.Models;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IRepositoryRead _readModel;

        private readonly IRepositoryWrite _writeModel;

        private readonly ILogger _logger;

        public MonitorUc(IRepositoryRead readModel, IRepositoryWrite writeModel, ILogger logger)
        {
            _readModel = readModel;
            _writeModel = writeModel;
            _logger = logger;
        }

        public async Task BeginProcessingHeartbeats()
        {
            await Try.WithConsequenceAsync(async () =>
            {
                var routes = _readModel.GetAllNodes();

                //if empty or null
                if ((routes?.Any() ?? true) is false) return Task.FromResult(Task.CompletedTask);

                await Parallel.ForEachAsync(routes!, async (node, _) => await Acknowledge(node));

                var unknownStateRoutes = routes!.Where(node =>
                {
                    node.ServiceStatus?.EvaluateState(node);

                    return node.ServiceStatus is UnknownState;
                }).ToList();

                foreach (var node in unknownStateRoutes)
                {
                    _logger.LogCritical($"Lost connection to {node.Name}, last known state {node.LastSuccessfulPing}");
                    _writeModel.RemoveNode(node);
                }

                return Task.FromResult(Task.CompletedTask);
            }, retryCount: int.MaxValue);
        }

        private async Task Acknowledge(INode node)
        {
            var route = _readModel.ReadNodeById(node.Name);

            if (route is not null)
            {
                if (await IsPortOpen(node.Address, Convert.ToInt32(node.Port)))
                    route.LastSuccessfulPing = DateTime.UtcNow;
            }

            static async Task<bool> IsPortOpen(string host, int port)
            {
                using var tcpClient = new TcpClient();

                try
                {
                    await tcpClient.ConnectAsync(host, port).WaitAsync(TimeSpan.FromMilliseconds(100));

                    return tcpClient.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
