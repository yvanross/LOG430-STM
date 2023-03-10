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
        private readonly IRepositoryWrite _repositoryWrite;
        
        private readonly IRepositoryRead _repositoryRead;

        private readonly IEnvironmentClient _environmentClient;

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
            var newService = new ServiceInstance()
            {
                Id = subscriptionDto.ServiceId,
                ContainerInfo = container,
                Address = subscriptionDto.ServiceAddress,
                Type = subscriptionDto.ServiceType,
            };

            var containerConfig = await _environmentClient.GetContainerConfig(container.Id);

            if (containerConfig is null)
                throw new Exception("container Config was null");

            var serviceType = new ServiceType()
            {
                ContainerConfig = containerConfig,
                Type = subscriptionDto.ServiceType,
                AutoScaleInstances = subscriptionDto.AutoScaleInstances,
                MinimumNumberOfInstances = subscriptionDto.MinimumNumberOfInstances
            };

            _repositoryWrite.UpdateServiceType(newService, serviceType);

            _repositoryWrite.WriteService(newService);
        }

    }
}
