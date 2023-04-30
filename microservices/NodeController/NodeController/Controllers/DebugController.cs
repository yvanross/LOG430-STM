using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger<DebugController> _logger;
        private readonly IPodReadService _readService;

        public DebugController(ILogger<DebugController> logger, IPodReadService readService)
        {
            _logger = logger;
            _readService = readService;
        }

        [HttpGet]
        [ActionName(nameof(GetPodTypes))]
        public List<IPodType> GetPodTypes()
        {
            return _readService.GetAllPodTypes().ToList();
        }

        [HttpGet]
        [ActionName(nameof(GetPodInstances))]
        public List<IPodInstance> GetPodInstances()
        {
            return _readService.GetAllPods().ToList();
        }
    }
}
