using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STM.Use_Cases;

namespace STM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisableService : ControllerBase
    {
        private readonly ILogger<DisableService> _logger;

        public DisableService(ILogger<DisableService> logger)
        {
            _logger = logger;
        }


        [HttpPost(Name = "Disable Service")]
        public void Post()
        {
            ChaosDaemon.ChaosEnabled = true;

            _logger.LogInformation("Service has been disabled");
        }
    }
}
