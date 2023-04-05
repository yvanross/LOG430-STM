using ApplicationLogic.Usecases;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NodeController.External.Docker;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class LogStoreController : ControllerBase
    {
        private readonly ILogger<LogStoreController> _logger;

        public LogStoreController(ILogger<LogStoreController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Post([FromRoute] string id)
        {
            try
            {
                _logger.LogInformation($"{id} attempting to get database");

                return Ok($"{HostInfo.ServiceAddress}:{HostInfo.NodeStateStoragePort}" );
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
