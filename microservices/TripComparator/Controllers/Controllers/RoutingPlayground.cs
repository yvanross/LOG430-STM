using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceMeshHelper;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Controllers;

namespace Controllers.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class RoutingPlayground : ControllerBase
{
    private readonly ILogger<RoutingPlayground> _logger;

    public RoutingPlayground(ILogger<RoutingPlayground> logger)
    {
        _logger = logger;
    }

    [ActionName("PingNamespace")]
    [HttpPost]
    public async Task<IActionResult> PingNamespace()
    {
        _logger.LogInformation($"pinging other");

        var res = await RestController.Get(new GetRoutingRequest()
        {
            Endpoint = "RoutingPlayground/Echo",
            TargetService = "Core.TripComparator",
            Mode = LoadBalancingMode.RoundRobin
        });

        await foreach (var echoRes in res.ReadAllAsync())
        {
            var echo = JsonConvert.DeserializeObject<ActionResult<string>>(echoRes.Content);

            _logger.LogInformation(echo.Value);
        }

        return Ok();
    }
    [ActionName("PingAll")]
    [HttpPost]
    public async Task<IActionResult> PingAll()
    {
        _logger.LogInformation($"pinging other");

        var res = await RestController.Get(new GetRoutingRequest()
        {
            Endpoint = "RoutingPlayground/Echo",
            TargetService = "TripComparator",
            Mode = LoadBalancingMode.Broadcast
        });

        await foreach (var echoRes in res.ReadAllAsync())
        {
            var echo = JsonConvert.DeserializeObject<ActionResult<string>>(echoRes.Content);

            _logger.LogInformation(echo.Value);
        }

        return Ok();
    }

    [ActionName("Echo")]
    [HttpGet]
    public ActionResult<string> Echo()
    {
        _logger.LogInformation($"Return Echo");

        return Ok("echo");
    }
}