using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authService)
        {
            _authService = authService;
            _logger = logger;
        }

       
        [HttpPost(Name = "Login")] 
        public async Task<ActionResult<ApiResponse<UserDto>>> Login([FromForm] UserDto auth)
        {
            try
            {
                var response = await _authService.Login(auth);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<UserDto>.ErrorResponse();
            }
        }
    }
}