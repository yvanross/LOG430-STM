using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TripComparator.DTO;
using TripComparator.External;

namespace TripComparator.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TripComparatorController : ControllerBase
    {
        private readonly ILogger<TripComparatorController> _logger;

        private readonly CompareTimesUC _compareTimesUc;

        public TripComparatorController(ILogger<TripComparatorController> logger)
        {
            _logger = logger;
            _compareTimesUc = new(new RouteTimeProviderClient(), new StmClient(_logger), new MassTransitRabbitMqClient());
        }

        [HttpPost]
        [ActionName(nameof(Post))]
        public async Task<ActionResult<int>> Post(string startingCoordinates, string destinationCoordinates)
        {
            _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

            var producer = await _compareTimesUc.BeginComparingBusAndCarTime(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            _ = _compareTimesUc.WriteToStream(producer);

            return Ok();

            string RemoveWhiteSpaces(string s)
                => s.Replace(" ", "");
        }

        [HttpPost]
        [ActionName(nameof(Debug))]
        public async Task<ActionResult<int>> Debug()
        {
            var mt = new MassTransitRabbitMqClient();

            await mt.BeginStreaming();

            await mt.Produce(new Saga()
            {
                Message = "test message",
                Seconds = 100
            });

            return Ok();

            string RemoveWhiteSpaces(string s)
                => s.Replace(" ", "");
        }
    }
}