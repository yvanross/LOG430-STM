using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Docker.DotNet.Models;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;
using System.Net.NetworkInformation;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {

        private readonly IRepositoryRead _readModel;

        private readonly IRepositoryWrite _writeModel;

        public MonitorUc(IRepositoryRead readModel, IRepositoryWrite writeModel)
        {
            _readModel = readModel;
            _writeModel = writeModel;
        }

        public void Acknowledge(INode node)
        {
            var route = _readModel.ReadNodeById(node.Name);

            if (route is not null)
            {
                Ping pingSender = new ();
                try
                {
                    var reply = pingSender.Send($"http://{route.Address}:{route.Port}");

                    if (reply.Status.Equals(IPStatus.Success))
                        route.LastSuccessfulPing = DateTime.UtcNow;
                }
                catch
                {
                    // ignored
                }
            }
        }

        private async Task BeginProcessingHeartbeats()
        {
            await Try.WithConsequenceAsync(() =>
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
                    _writeModel.RemoveNode(node);
                }

                return Task.FromResult(Task.CompletedTask);
            }, retryCount: int.MaxValue);
        }
    }
}
