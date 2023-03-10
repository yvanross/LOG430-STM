using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambassador.Dto;
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
            return _repositoryRead.ReadServiceByAddressAndPort(ipAddress, portNumber) is not null;
        }

        public void Subscribe(SubscriptionDto subscriptionDto, ContainerInfo container)
        {
            _repositoryWrite.Write(new Service()
            {
                Id = subscriptionDto.ServiceId,
                ContainerInfo = container,
                Address = subscriptionDto.ServiceAddress,
                ServiceType = subscriptionDto.ServiceType,
            });
        }
    }
}
