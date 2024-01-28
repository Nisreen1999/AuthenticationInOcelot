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
            // Assuming you have the token as a string
            string token = result.Token;

            // Create a HttpClient for sending the request
            HttpClient client = new HttpClient();

            // Add the token to the Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // The URL of the target microservice
            string url = "https://localhost:7035/api/Courses/x";

            // Send the request
            HttpResponseMessage response = await client.GetAsync(url);

            // Check the response
            if (response.IsSuccessStatusCode)
            {
                // The request was successful
                string responseContent = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // The request failed
            }
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
            //HttpResponseMessage response = await client.GetAsync("https://localhost:7048");
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
