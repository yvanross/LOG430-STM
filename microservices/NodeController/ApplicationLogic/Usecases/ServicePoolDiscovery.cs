using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.Extensions;
using Microsoft.Extensions.Logging;
using MqContracts;

namespace ApplicationLogic.Usecases
{
    public class ServicePoolDiscovery
    {
        public static ImmutableHashSet<string> BannedIds = ImmutableHashSet<string>.Empty;

        private readonly IPodWriteService _podWriteService;
        private readonly IPodReadService _podReadService;
        private readonly IEnvironmentClient _environmentClient;
        private readonly ILogger _logger;
        private readonly IHostInfo _hostInfo;

        public ServicePoolDiscovery(IPodWriteService podWriteService, IPodReadService podReadService, IEnvironmentClient environmentClient, ILogger<ServicePoolDiscovery> logger, IHostInfo hostInfo)
        {
            _podWriteService = podWriteService;
            _podReadService = podReadService;
            _environmentClient = environmentClient;
            _logger = logger;
            _hostInfo = hostInfo;
        }

        public async Task DiscoverServices()
        {
            var unregisteredServices = await GetUnregisteredServices();

            var containerInfos = (await Task.WhenAll(unregisteredServices.ConvertAll(async unregisteredService =>
            {
                var containerConfig = await _environmentClient.GetContainerInfo(unregisteredService);

                try
                {
                    return new ServiceConfigurationWrapper(containerConfig.CuratedInfo, containerConfig.RawConfig);
                }
                catch (Exception e)
                {
                    BanId(e, containerConfig.CuratedInfo.Id);
                }

                return null;
            })))
                .OfType<ServiceConfigurationWrapper>()
                .ToList();

            foreach (var container in containerInfos.OrderByDescending(container => container.PodLinks.Count()))
            {
                _logger.LogInformation($"Registering service {container.CuratedInfo.Name} with {container.PodLinks.Count()} links");
            }

            //sorting by the number of links (sidecars), so that the services with the most links are registered first, to avoid creating pods that are not needed
            foreach (var container in containerInfos.OrderByDescending(container => container.PodLinks.Count()))
            {
                try
                {
                    string podId;

                    if (ServiceAlreadyKnown(container, out var registeredPodInstance))
                    {
                        podId = registeredPodInstance!.Id;
                    }
                    else
                    {
                        if (_podReadService.GetServiceType(container.ArtifactName) is not { } serviceType) 
                            serviceType = CreateServiceType(container);

                        if (_podReadService.GetPodType(container.PodName) is null)
                            _ = CreatePodType(container, serviceType);

                        var podInstance = GetPodWhereThisInstanceIsNeededToCompletePodInstance(serviceType);

                        podId = podInstance?.Id ?? Guid.NewGuid().ToString();
                    }
                    
                    var serviceInstance = CreateService(container, podId);

                    RegisterOrUpdatePodInstance(container, serviceInstance, podId);

                    MergeVirtualPodTypesNeededInThisPod(podId);
                }
                catch (Exception e)
                {
                    BanId(e, container.CuratedInfo.Id);
                }
            }

            bool ServiceAlreadyKnown(ServiceConfigurationWrapper container, out IPodInstance? podInstance)
            {
                var registeredService = _podReadService.GetServiceById(container.ServiceId);

                if (registeredService is not null)
                {
                    var registeredPodInstance = _podReadService.GetPodOfService(registeredService);

                    if (registeredPodInstance is not null)
                    {
                        podInstance = registeredPodInstance;

                        return true;
                    }
                }

                podInstance = null;

                return false;
            }

            IPodInstance? GetPodWhereThisInstanceIsNeededToCompletePodInstance(IServiceType serviceType)
            {
                var podTypesUsingThisService = _podReadService.GetAllPodTypes()
                    .Where(pt => pt.ServiceTypes.Exists(st => st.Type.EqualsIgnoreCase(serviceType.Type))).ToList();

                var podWhereThisServiceIsNeeded = podTypesUsingThisService
                    .SelectMany(pt => _podReadService.GetPodInstances(pt.Type)
                        .Where(podInstance => podInstance.ServiceStatus is LaunchedState && 
                                              podInstance.ServiceInstances.All(serviceInstance => serviceInstance.Type.EqualsIgnoreCase(serviceType.Type) is false)))
                    .FirstOrDefault();

                return podWhereThisServiceIsNeeded;
            }
        }

        private void MergeVirtualPodTypesNeededInThisPod(string podId)
        {
            var newPodInstance = _podReadService.GetPodById(podId);

            if (newPodInstance is null) throw new Exception("Pod should be registered");

            var newPodType = _podReadService.GetPodType(newPodInstance.Type);

            if (newPodType is null) throw new Exception("PodType should be registered");

            var missingServices = newPodType.ServiceTypes
                .Where(st => newPodInstance.ServiceInstances
                    .All(si => si.Type.EqualsIgnoreCase(st.Type) is false));

            if (missingServices.Any())
            {
                var virtualPodTypesNeededInThisPod = _podReadService.GetAllPodTypes()
                    .Where(podType =>
                        podType.ServiceTypes.Count.Equals(1) &&
                        podType.NumberOfInstances.Equals(0) &&
                        podType.Type.Equals(podType.ServiceTypes.First().Type) &&
                        missingServices.Any(st => st.Type.EqualsIgnoreCase(podType.Type)))
                    .ToList();

                if (virtualPodTypesNeededInThisPod.Any())
                {
                    foreach (var virtualPodType in virtualPodTypesNeededInThisPod)
                    {
                        var virtualPodInstance = _podReadService.GetServiceInstances(virtualPodType.ServiceTypes.Single().Type).Single();

                        newPodInstance.AddServiceInstance(virtualPodInstance);

                        _podWriteService.RemovePodType(virtualPodType);
                    }

                    _podWriteService.AddOrUpdatePod(newPodInstance);
                }
            }
        }

        private void BanId(Exception e, string id)
        {
            _logger.LogCritical(e, "Invalid Service configuration found in container pool");

            ImmutableInterlocked.Update(ref BannedIds, (set) => set.Add(id));
        }

        private void RegisterOrUpdatePodInstance(ServiceConfigurationWrapper container, IServiceInstance newService, string podId)
        {
            var pod = _podReadService.GetPodById(podId) ?? new PodInstance(_podReadService)
            {
                ServiceInstances = ImmutableList<IServiceInstance>.Empty,
                Type = container.PodName,
                Id = podId,
                ServiceStatus = new LaunchedState()
            };

            pod.ReplaceServiceInstance(newService);

            _podWriteService.AddOrUpdatePod(pod);
        }

        private ServiceInstance CreateService(ServiceConfigurationWrapper container, string newPodId)
        {
            var volumes = container.RawConfig.Config.Mounts.Select(v => v.Name).ToList();

            var newService = new ServiceInstance
            {
                Id = container.ServiceId,
                ContainerInfo = container.CuratedInfo,
                Address = _hostInfo.GetAddress(),
                Type = container.ArtifactName,
                PodId = container.PodId ?? newPodId,
                ServiceStatus = new ReadyState(),
                VolumeIds = volumes,
            };

            return newService;
        }

        private IPodType CreatePodType(ServiceConfigurationWrapper container, IServiceType serviceType)
        {
            var newPodType = new PodType(_podReadService)
            {
                Type = container.PodName,
                ShareVolumes = container.ShareVolumesWithReplicas
            };

            newPodType.SetPodLeader(serviceType.Type);

            if (container.NumberOfInstancesAsNumber is { } number and > 0)
                newPodType.SetNumberOfPod(number);

            else if (container.NumberOfInstancesAsHostNames is { } hostNames)
                newPodType.AddRangeHostnames(hostNames);

            var serviceTypes = container.PodLinks.ToList();

            serviceTypes.Add(serviceType.Type);

            newPodType.AddRangeServiceTypes(serviceTypes);

            _podWriteService.AddOrUpdatePodType(newPodType);

            return newPodType;
        }

        private IServiceType CreateServiceType(ServiceConfigurationWrapper container)
        {
            var serviceType = new ServiceType()
            {
                ContainerConfig = container.RawConfig,
                Type = container.ArtifactName,
                ArtifactType = container.ArtifactCategory,
                DnsAccessibilityModifier = container.PodLinks.Any() ? Enum.GetName(AccessibilityModifierEnum.Public)! : container.Dns,
            };

            _podWriteService.AddOrUpdateServiceType(serviceType);

            return serviceType;
        }

        private async Task<List<string>> GetUnregisteredServices()
        {
            var runningServicesIds = await _environmentClient.GetRunningServices();

            var registeredServices = _podReadService.GetAllServices()
                .Where(s => s.ContainerInfo is not null && s.ServiceStatus is not LaunchedState).DistinctBy(s => s.Id).ToDictionary(s => s.ContainerInfo!.Id);

            var filterServices = runningServicesIds?.Where(runningService =>
                BannedIds.Contains(runningService) is false &&
                registeredServices.ContainsKey(runningService) is false &&
                runningService.Equals(_hostInfo.GetContainerId()) is false).ToList();

            return filterServices ?? new List<string>();
        }
    }
}
