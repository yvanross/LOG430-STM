using ApplicationLogic.Dto;
using ApplicationLogic.Usecases;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MqContracts;
using Monitor = ApplicationLogic.Usecases.Monitor;

namespace Ingress.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartBeatIngressControllerMq : IConsumer<HeartBeatDto>
    {
        private readonly ILogger<HeartBeatIngressControllerMq> _logger;
        private readonly Monitor _monitor;

        public HeartBeatIngressControllerMq(ILogger<HeartBeatIngressControllerMq> logger, Monitor monitor)
        {
            _logger = logger;
            _monitor = monitor;
        }

        public Task Consume(ConsumeContext<HeartBeatDto> context)
        {
            _monitor.TryAcknowledge(
                context.Message.Source,
                context.Message.Version,
                context.Message.Secure,
                context.Message.Dirty);

            return Task.CompletedTask;
        }
    }
}