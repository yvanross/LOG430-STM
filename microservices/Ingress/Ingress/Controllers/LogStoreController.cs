using ApplicationLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ingress.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogStoreController : ControllerBase
    {
        private readonly ILogger<LogStoreController> _logger;
        private readonly IHostInfo _hostInfo;

        public LogStoreController(ILogger<LogStoreController> logger, IHostInfo hostInfo)
        {
            _logger = logger;
            _hostInfo = hostInfo;
        }

        [HttpGet("{id}")]
        public IActionResult Post([FromRoute] string id)
        {
            try
            {
                _logger.LogInformation($"{id} attempting to get database");

                return Ok($"{_hostInfo.GetAddress()}:{_hostInfo.GetNodeStateStoragePort()}");
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.Message} \n \t{e.StackTrace}";

                _logger.LogError(errorMessage);

                return Problem(errorMessage);
            }
        }
    }
}
