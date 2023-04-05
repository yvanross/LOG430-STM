using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TripComparator.DTO;
using TripComparator.External;

namespace TripComparator.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger<DebugController> _logger;

        private readonly CompareTimesUC _compareTimesUc;

        public DebugController(ILogger<DebugController> logger)
        {
            _logger = logger;
            _compareTimesUc = new(new RouteTimeProviderClient(), new StmClient(_logger), null, logger);
        }

        [HttpPost]
        [EndpointDescription("This endpoint is here so you may manually test your system, it's for 'fun'.")]
        public async Task<ActionResult<int>> Post(string startingCoordinates, string destinationCoordinates)
        {
            _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

            var producer = await _compareTimesUc.BeginComparingBusAndCarTime(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            _ = _compareTimesUc.WriteToStream(producer);

            return Ok();

            string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
        }
    }
}