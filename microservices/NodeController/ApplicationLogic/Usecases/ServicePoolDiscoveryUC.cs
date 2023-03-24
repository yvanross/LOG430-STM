using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace ApplicationLogic.Usecases
{
    public class ServicePoolDiscoveryUC
    {
        private readonly IPodWriteModel _podWriteModel;
        
        private readonly IPodReadModel _podReadModel;

        private readonly IEnvironmentClient _environmentClient;

        private static string NodeAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

        public ServicePoolDiscoveryUC(IPodWriteModel podWriteModel, IPodReadModel podReadModel, IEnvironmentClient environmentClient)
        {
            _podWriteModel = podWriteModel;
            _podReadModel = podReadModel;
            _environmentClient = environmentClient;
        }

        public async Task DiscoverServices()
        {
            var unregisteredServices = await GetUnregisteredServices();

            foreach (var unregisteredService in unregisteredServices!)
            {
                var service = await _environmentClient.GetContainerInfo(unregisteredService);

                var newService = CreateService(service);

                CreateServiceType(service);

                CreateOrUpdatePodInstance(service, newService);
            }

            async Task<List<string>?> GetUnregisteredServices()
            {
                var runningServicesIds = await _environmentClient.GetRunningServices();

                var registeredServices = _podReadModel.GetAllServices().ToDictionary(s => s.Id);

                var unregisteredServices = runningServicesIds?.Where(runningService => registeredServices.ContainsKey(runningService) is false).ToList();
                
                return unregisteredServices;
            }

            ServiceInstance CreateService((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service)
            {
                var newService = new ServiceInstance()
                {
                    Id = service.RawConfig.Config.Config.Env.First(e => e.ToString().StartsWith("ID=")),
                    ContainerInfo = service.CuratedInfo,
                    Address = NodeAddress,
                    Type = service.CuratedInfo.Name,
                    PodId = GetPodId(service.CuratedInfo.Labels)
                };

                newService.ServiceStatus = new ReadyState();

                return newService;
            }

            void CreateServiceType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service)
            {
                var curatedInfoLabels = service.CuratedInfo.Labels;

                var podType = GetPodName(curatedInfoLabels);

                var serviceType = new ServiceType()
                {
                    ContainerConfig = service.RawConfig,
                    Type = service.CuratedInfo.Name,
                    ArtifactType = GetComponentCategory(curatedInfoLabels),
                    IsPodSidecar = GetIsSidecar(curatedInfoLabels),
                    PodName = podType
                };

                UpdateOrCreatePodType(podType, service, serviceType);
            }

            void UpdateOrCreatePodType(string podType, (ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceType serviceType)
            {
                var pod = _podReadModel.GetPodType(podType);

                if (pod is null)
                {
                    _podWriteModel.AddOrUpdatePodType(new PodType()
                    {
                        Type = podType,
                        MinimumNumberOfInstances = GetMinimumNumberOfInstances(service.CuratedInfo.Labels),
                        Gateway = GetIsSidecar(service.CuratedInfo.Labels) ? serviceType : pod?.Gateway ?? default,
                        ServiceTypes = pod?.ServiceTypes.Add(serviceType) ?? ImmutableList<IServiceType>.Empty.Add(serviceType),
                    });
                }
            }

            void CreateOrUpdatePodInstance((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceInstance newService)
            {
                var podId = GetPodId(service.CuratedInfo.Labels);

                var pod = _podReadModel.GetPodById(podId);

                pod ??= new PodInstance()
                {
                    ServiceInstances = pod?.ServiceInstances?.Add(newService) ??
                                       ImmutableList<IServiceInstance>.Empty.Add(newService),
                    Type = GetPodName(service.CuratedInfo.Labels),
                    Id = podId
                };

                _podWriteModel.AddOrUpdatePod(pod);
            }
        }

        private string GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            labels.TryGetValue(serviceLabels, out var label);

            return label ?? string.Empty;
        }

        private string GetComponentCategory(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.COMPONENT_CATEGORY, labels);

            return string.IsNullOrEmpty(value) ? nameof(ArtifactTypeEnum.Undefined) : value;
        }

        private int GetMinimumNumberOfInstances(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            uint.TryParse(GetLabelValue(ServiceLabelsEnum.MINIMUM_NUMBER_OF_INSTANCES, labels), out var nbInstances);
            
            return Convert.ToInt32(nbInstances);
        }

        private string GetPodName(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_NAME, labels);
        }

        private string GetPodId(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_ID, labels);
        }

        private bool GetIsSidecar(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            bool.TryParse(GetLabelValue(ServiceLabelsEnum.IS_POD_SIDECAR, labels), out var isSidecar);

            return isSidecar;
        }
    }
}
