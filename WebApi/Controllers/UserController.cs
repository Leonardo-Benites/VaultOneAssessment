using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUser/{id}")]
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

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] UserDto user)
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
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Put(int id, [FromBody] UserDto UserDto)
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
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
