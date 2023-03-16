using Ambassador.Dto;
using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NodeController.Docker;
using NodeController.Repository;

namespace NodeController.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/{source}/[action]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUc;

        private readonly MonitorUc _monitorUc;

        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;

            var source = httpContextAccessor.HttpContext!.GetRouteValue("source")!.ToString();

            var _readModel = new RepositoryRead(source);
            var _writeModel = new RepositoryWrite();
            var _environmentClient = new LocalDockerClient(logger);

            _subscriptionUc = new SubscriptionUC(_writeModel, _readModel, _environmentClient);
            _monitorUc = new(_environmentClient, _readModel, _writeModel);
        }

        [HttpPut]
        [ActionName(nameof(Subscribe))]
        public async Task<IActionResult> Subscribe(SubscriptionDto subscriptionDto)
        {
            try
            {
                _logger.LogInformation($"{subscriptionDto.ServiceType} attempting to subscribe");

                var container = await _monitorUc.GetPort(subscriptionDto.ContainerId);

                if (string.IsNullOrEmpty(container.Port)) throw new Exception("Source port couldn't be determined");

                _logger.LogInformation($"Calling port: {container.Port}");

                await _subscriptionUc.Subscribe(subscriptionDto, container);

                _monitorUc.TryScheduleHeartBeatOnScheduler();

                _logger.LogInformation($"{subscriptionDto.ServiceType} from {subscriptionDto.ServiceAddress}:{container.Port} has subscribed");
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.Message} \n \t{e.StackTrace}";

                _logger.LogError(errorMessage);

                return Problem(errorMessage);
            }

            return Ok();
        }
    }
}