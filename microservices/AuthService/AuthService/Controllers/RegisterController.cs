using AuthService.Dto;
using AuthService.Services;
using Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;

        private readonly UserManager<LabUser> _userManager;

        public RegisterController(ILogger<RegisterController> logger, UserManager<LabUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("{teamName}/{username}")]
        public async Task<ActionResult> Post([FromRoute] string teamName, [FromRoute] string username, [FromBody] SecretDto payload)
        {
            _logger.LogInformation($"trying to register {username} of {teamName}");

            var secret = payload.Secret;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userManager.CreateAsync(new LabUser(username, teamName, "User"), secret);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Created("", null);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }
    }
}