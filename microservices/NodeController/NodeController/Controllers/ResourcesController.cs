using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly ILogger<ResourcesController> _logger;
        private readonly PlannedResourcesUpdate _plannedResources;

        public ResourcesController(ILogger<ResourcesController> logger, PlannedResourcesUpdate plannedResources)
        {
            _logger = logger;
            _plannedResources = plannedResources;
        }

        [HttpPost("{podTypeName}/IncreaseNumberOfPod")]
        public void IncreaseNumberOfPod([FromRoute] string podTypeName)
        {
            _plannedResources.IncreaseNumberOfPod(podTypeName);
        }

        [HttpPost("{podTypeName}/DecreaseNumberOfPod")]
        public void DecreaseNumberOfPod([FromRoute] string podTypeName)
        {
            _plannedResources.DecreaseNumberOfPod(podTypeName);
        }

        [HttpPost("{podTypeName}/SetNumberOfPod")]
        public void SetNumberOfPod([FromRoute] string podTypeName, int numberOfInstances)
        {
            _plannedResources.SetNumberOfPod(podTypeName, numberOfInstances);
        }

        [HttpPost(nameof(SetGlobalCpu))]
        public void SetGlobalCpu(long nanoCpus)
        {
            _plannedResources.SetGlobalCpu(nanoCpus);
        }
    }
}
