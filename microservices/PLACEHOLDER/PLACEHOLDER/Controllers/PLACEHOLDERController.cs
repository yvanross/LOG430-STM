using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace PLACEHOLDER.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class PLACEHOLDERController : ControllerBase
    {
        private readonly ILogger<PLACEHOLDERController> _logger;

        public PLACEHOLDERController(ILogger<PLACEHOLDERController> logger)
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