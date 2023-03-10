using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambassador.Dto;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases
{
    public class SubscriptionUC
    {
        private IRepositoryWrite _repositoryWrite;
        
        private IRepositoryRead _repositoryRead;

        private IEnvironmentClient _environmentClient;

        public SubscriptionUC(IRepositoryWrite repositoryWrite, IRepositoryRead repositoryRead, IEnvironmentClient environmentClient)
        {
            _repositoryWrite = repositoryWrite;
            _repositoryRead = repositoryRead;
            _environmentClient = environmentClient;
        }

        public bool CheckIfServiceIsSubscribed(string ipAddress, string portNumber)
        {
            return _repositoryRead.ReadServiceByAddressAndPort(ipAddress, portNumber) is not null;
        }

        public async Task Subscribe(SubscriptionDto subscriptionDto, ContainerInfo container)
        {
            var newService = new Service()
            {
                Id = subscriptionDto.ServiceId,
                ContainerInfo = container,
                Address = subscriptionDto.ServiceAddress,
                ServiceType = subscriptionDto.ServiceType,
            };

            //missing logic to sent a minimum number of containers of a certain type
            if (subscriptionDto.AutoScaleInstances)
            {
                var containerConfig = await _environmentClient.GetContainerConfig(container.Id);

                if (containerConfig is null)
                    throw new Exception("container Config was null");

                _repositoryWrite.UpdateContainerModel(newService, containerConfig);
            }

            _repositoryWrite.WriteService(newService);
        }

    }
}
