using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUsers()
        {
            try
            {
                var response = await _userService.GetAll();

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

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            try
            {
                var response = await _userService.GetById(id);

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

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromForm] UserDto user)
        {
            try
            {
                var response = await _userService.Create(user);

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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Put(int id, [FromForm] UserDto UserDto)
        {
            try
            {
                var response = await _userService.Update(id, UserDto);

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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Delete(int? id)
        {
            try
            {
                var response = await _userService.Delete(id);

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
