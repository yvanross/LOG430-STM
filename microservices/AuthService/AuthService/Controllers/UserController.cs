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
                var substring = "Bearer ";

                jwt = authorizationHeader.ToString().Length > substring.Length ? authorizationHeader.ToString().Substring(substring.Length).Trim() : string.Empty;
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return Ok(_userManager.Users.Select(u => u.UserName).ToArray());
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
                users = _userManager.Users.Where(u=>u.Role.Equals("User")).ToList();
            }
            else
            {
                users.Add(await _userManager.FindByNameAsync(user));
            }

            return Ok(users.Where(u => u.Group.Equals(group)).Select(u=>u.UserName).ToArray());
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete([FromRoute] string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user is not null && user.Role.Equals("Admin") is false)
            {
                var result = await _userManager.DeleteAsync(user);

                return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
            }

            return Ok();
        }


    }
}