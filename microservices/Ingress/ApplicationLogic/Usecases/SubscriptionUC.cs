using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;

namespace ApplicationLogic.Usecases
{
    public class SubscriptionUC
    {
        private IRepositoryWrite _repositoryWrite;
        
        private IRepositoryRead _repositoryRead;

        public SubscriptionUC(IRepositoryWrite repositoryWrite, IRepositoryRead repositoryRead)
        {
            _repositoryWrite = repositoryWrite;
            _repositoryRead = repositoryRead;
        }

        public bool CheckIfServiceIsSubscribed(string ipAddress, string portNumber)
        {
            return _repositoryRead.ReadRouteByAddressAndPort(ipAddress, portNumber) is not null;
        }

        public void Subscribe(string serviceAddress, string port, string serviceType)
        {
            _repositoryWrite.Write(new Route()
            {
                Address = serviceAddress,
                PortNumber = port,
                ServiceType = serviceType,
                Id = Guid.NewGuid().ToString() 
            });
        }
    }
}
