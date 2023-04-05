using System.Runtime.CompilerServices;
using ApplicationLogic.Usecases;
using Ingress.Dto;
using Ingress.Extensions;
using Ingress.External;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly ILogger<IngressController> _logger;

        private readonly RoutingUC _routingUc;

        public IngressController(ILogger<IngressController> logger)
        {
            _logger = logger;

            var repoRead = new RepositoryRead();
            this._routingUc = new RoutingUC(repoRead);
        }

        [HttpPost, 
         ActionName(nameof(BeginExperiment)),
         SwaggerOperation("Sends a request to the node controllers (kubelets) connected to this ingress to begin a new chaos experiment.",""" 
            This endpoint takes a json body of the same format as the NodeController Begin endpoint.
            It simply forwards the body to the right client (teams).
        """)]
        public async Task<IActionResult> BeginExperiment([FromQuery] string[] targetTeams)
        {
            var requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            await Parallel.ForEachAsync(targetTeams, async (team, _) =>
            {
                try
                {
                    var route = _routingUc.RouteByDestinationType(team);

                    var nodeController = new NodeControllerClient(route, _logger);

                    await nodeController.BeginExperiment(requestBody);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            });

            return Ok();
        }
    }
}
