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
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger;

        private readonly UserManager<LabUser> _userManager;

        private readonly JwtService _jwtService;

        public AuthorizeController(ILogger<AuthorizeController> logger, UserManager<LabUser> userManager, JwtService jwtService)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("{userName}")]
        public async Task<ActionResult<AuthResponse>> Post([FromRoute] string username, [FromBody] SecretDto payload)
        {
            _logger.LogInformation($"trying to authorize {username}");

            var secret = payload.Secret;

            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, secret);

            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var token = _jwtService.CreateToken(user);

            return Ok(token.Token);
        }
    }
}