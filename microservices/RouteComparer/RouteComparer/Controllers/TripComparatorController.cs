using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TripComparator.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TripComparatorController : ControllerBase
    {
        private readonly ILogger<TripComparatorController> _logger;

        private readonly CompareTimesUC compareTimesUc = new();

        public TripComparatorController(ILogger<TripComparatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(Get))]
        public async Task<ActionResult<int>> Get(string startingCoordinates, string destinationCoordinates)
        {
            var travelTime = await compareTimesUc.CompareBusAndCarTime(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            return Ok(travelTime);

            string RemoveWhiteSpaces(string s)
                => s.Replace(" ", "");
        }
    }
}