using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Monitor.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MonitorController : ControllerBase
    {
        private readonly ILogger<MonitorController> _logger;

        public MonitorController(ILogger<MonitorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ActionName(nameof(BeginMonitoring))]
        public int BeginMonitoring([FromQuery] string portainerAddress)
        {


            return 0;
        }
    }
}