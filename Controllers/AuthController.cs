using AuthenticationAutherization.data;
using AuthenticationAutherization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AuthenticationAutherization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService; 
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);
            return Ok(new { token = result.Token });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }
            var result = await _authService.RegisterAsync(model);
            if(!result.ISAuthenticated)
            {
                return BadRequest(result.Message);
            }
            return Ok(new {token = result.Token});
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public List<IdentityRole> GetAllRoles()
        {
          return _authService.GetAllRoles();
        }

    }
}
