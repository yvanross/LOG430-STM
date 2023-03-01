using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases
{
    public class MonitorUc
    {
        private readonly IEnvironmentClient _client;

        public MonitorUc(IEnvironmentClient client)
        {
            _client = client;
        }
        public async Task<List<Microservice>> GetRunningMicroservices()
        {
            return await _client.GetRunningContainersIds();
        }
    }
}
