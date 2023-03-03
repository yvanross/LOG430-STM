using Microsoft.AspNetCore.Mvc;
using STM.Use_Cases;

namespace STM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnableService : ControllerBase
    {
        private readonly ILogger<EnableService> _logger;

        public EnableService(ILogger<EnableService> logger)
        {
            _logger = logger;
        }


        [HttpPost(Name = "Enable Service")]
        public void Post()
        {
            ChaosDaemon.ChaosEnabled = false;

            _logger.LogInformation("Service has been enabled");
        }
    }
}
