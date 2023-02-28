using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace PLACEHOLDER.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RouteTimeProviderController : ControllerBase
    {
        private readonly ILogger<RouteTimeProviderController> _logger;

        public RouteTimeProviderController(ILogger<RouteTimeProviderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(Get))]
        public int Get()
        {
            return 0;
        }
    }
}