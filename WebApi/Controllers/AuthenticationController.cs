using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<dynamic>>> Login([FromBody] AuthDto auth)
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
                return ApiResponse<dynamic>.ErrorResponse();
            }
        }
    }
}