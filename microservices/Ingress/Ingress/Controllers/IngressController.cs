﻿using System.IdentityModel.Tokens.Jwt;
using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ApplicationLogic.Dto;
using Entities.DomainInterfaces;
using ApplicationLogic.Interfaces.Dao;
using System.Security.Principal;
using System.Linq;

namespace Ingress.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly ILogger<IngressController> _logger;
        private readonly IDataStream _dataStream;
        private readonly Subscription _subscription;
        private readonly ISystemStateReadService _systemStateReadService;

        public IngressController(ILogger<IngressController> logger, IDataStream dataStream, ISystemStateReadService systemStateReadService,
                                    Subscription subscription)
        {
            _logger = logger;
            _dataStream = dataStream;
            _subscription = subscription;
            _systemStateReadService = systemStateReadService;
        }

        [HttpPost, 
         ActionName(nameof(BeginExperiment)),
         SwaggerOperation("Sends a request to the node controllers (kubelets) connected to this ingress to begin a new chaos experiment.",
             """ 
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
                    await _dataStream.Produce(team, requestBody);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            });

            return Ok();
        }

        [HttpPost,
         ActionName(nameof(Authorize)),
         SwaggerOperation("Authorizes or invalidates the sent credential if a user with the same name exists. Otherwise, it creates a new user with the sent credentials"
        )]
        public async Task<IActionResult> Authorize([FromBody] IdentityDto identity)
        {
            var role = await _subscription.Authorize(identity.Name, identity.Secret);

            if (role is null)
            {
                return Unauthorized();
            }

            return Ok(role);
        }

        [HttpGet,
         ActionName(nameof(GetLogs)),
         SwaggerOperation("Authorizes or invalidates the sent credential if a user with the same name exists. Otherwise, it creates a new user with the sent credentials"
         )]
        public async Task<Dictionary<string, IEnumerable<object?>>> GetLogs()
        {
            var jwt = string.Empty;

            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                jwt = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return null; //BadRequest("No JWT provided");
            }

            var accounts = await _subscription.GetVisibleAccounts(jwt);

            var jwtToken = new JwtSecurityToken(jwt);

            // Assuming the role, group, and user are stored with standard claim types
            var group = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid)?.Value;

            if (group is null) return null; //Unauthorized("Group couldn't be parsed");

            var logs = await _systemStateReadService.ReadLogs(accounts, group);

            var logDictionary = logs.ToDictionary(kv => (kv.Key), kv => kv.Value);

            return logDictionary;
        }
    }
}
