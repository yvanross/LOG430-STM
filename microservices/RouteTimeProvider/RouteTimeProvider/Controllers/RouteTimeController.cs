using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace PLACEHOLDER.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RouteTimeController : ControllerBase
    {
        private readonly ILogger<RouteTimeController> _logger;

        private readonly TravelUC _travelUc = new ();

        public RouteTimeController(ILogger<RouteTimeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(Get))]
        public async Task<ActionResult<int>> Get(string startingCoordinates, string destinationCoordinates)
        {
            var travelTime = await _travelUc.GetTravelTimeInSeconds(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            return Ok(travelTime);

            string RemoveWhiteSpaces(string s)
                => s.Replace(" ", "");
        }
    }
}