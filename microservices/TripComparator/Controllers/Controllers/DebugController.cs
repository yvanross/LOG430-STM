using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger<DebugController> _logger;

        private readonly CompareTimes _compareTimes;

        public DebugController(ILogger<DebugController> logger, CompareTimes compareTimes)
        {
            _logger = logger;
        }

        [HttpPost]
        [EndpointDescription("This endpoint is here so you may manually test your system, it's for 'fun'.")]
        public async Task<ActionResult<int>> Post(string startingCoordinates, string destinationCoordinates)
        {
            _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

            var producer = await _compareTimes.BeginComparingBusAndCarTime(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            _ = _compareTimes.WriteToStream(producer);

            return Ok();

            string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
        }
    }
}