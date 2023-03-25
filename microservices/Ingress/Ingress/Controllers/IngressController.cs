using ApplicationLogic.Usecases;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> _logger;

        public IngressController(ILogger<SubscriptionController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
        }
    }
}
