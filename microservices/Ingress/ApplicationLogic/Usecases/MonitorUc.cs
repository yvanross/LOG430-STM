using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.Services;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IEnvironmentClient _client;

        private readonly HeartBeatService _heartbeatService;

        public MonitorUc(IEnvironmentClient client, HeartBeatService heartbeatService)
        {
            _client = client;
            _heartbeatService = heartbeatService;
        }

        public void ReceiveHeartBeat(Guid sender)
        {
            _heartbeatService.Acknowledge(sender);
        }

        public async Task<List<ContainerInfo>> GetRunningMicroservices()
        {
            return await _client.GetRunningServices();
        }

        public async Task<ContainerInfo> GetPort(string containerId)
        {
            var runningServices = await _client.GetRunningServices();

            var targetService = runningServices.SingleOrDefault(rs => rs.Id.Equals(containerId));

            return targetService;
        }

        public async Task IncreaseByOneNumberOfInstances(string containerId, string newContainerName)
        {
            await _client.IncreaseByOneNumberOfInstances(containerId, newContainerName);
        }

        public async Task RemoveContainerInstance(string containerId)
        {
            await _client.RemoveContainerInstance(containerId);
        }
    }
}
