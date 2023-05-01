using AuthService.Dto;
using AuthService.Services;
using Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        private readonly UserManager<LabUser> _userManager;

        public UserController(ILogger<UserController> logger, UserManager<LabUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<string[]>> Get()
        {
            var jwt = string.Empty;

            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                jwt = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return BadRequest("No JWT provided");
            }

            var jwtToken = new JwtSecurityToken(jwt);

            // Assuming the role, group, and user are stored with standard claim types
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var group = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid)?.Value;
            var user = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;


            IList<LabUser> users = new List<LabUser>();

            _logger.LogInformation($"trying to get all user names of group");

            if (role.Equals("Admin"))
            {
                users = await _userManager.GetUsersInRoleAsync("User");
            }
            else
            {
                users.Add(await _userManager.FindByNameAsync(user));
            }

            return Ok(users.Where(u => u.Group.Equals(group)).Select(u=>u.UserName).ToArray());
        }
    }
}