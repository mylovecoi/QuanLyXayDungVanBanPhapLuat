using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using System.ComponentModel.DataAnnotations;

namespace Base.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateTokenController : ControllerBase
    {
        private readonly IAuthService _authService;
        public GenerateTokenController(IAuthService authService)
        {
            _authService = authService;
        }
        public class LoginRequest
        {
            [Required(ErrorMessage = "Username is required")]
            public required string Username { get; set; }
            [Required(ErrorMessage = "Password is required")]
            public required string Password { get; set; }
        }

        [HttpPost]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { Message = "Username and password are required." });
                }

                return Ok(new { Token = _authService.GenerateToken(request.Username, request.Password) });
            }
            catch (Exception ex)
            {
                // Log the exception here (ex)
                //return StatusCode(500, "An error occurred while generating the token.");
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("GetTokenForAgent")]
        public IActionResult GetTokenForAgent([FromForm] string agentId)
        {
            try
            {
                return Ok(new { Token = _authService.GenerateTokenForAgent(agentId) });
            }
            catch (Exception ex)
            {
                // Log the exception here (ex)
                //return StatusCode(500, "An error occurred while retrieving the token.");
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
