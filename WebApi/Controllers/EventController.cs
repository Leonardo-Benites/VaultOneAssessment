using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet(Name = "GetPublicEvents")]
        public async Task<ActionResult<ApiResponse<EventDto>>> GetPublicEvents([FromQuery] EventDto eventDto)
        {
            try
            {
                var response = await _eventService.GetPublicEvents(eventDto);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<EventDto>.ErrorResponse();
            }
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetMyEvents")]
        public async Task<ActionResult<ApiResponse<EventDto>>> GetEventsByUserId(int userId, [FromQuery] EventDto eventDto)
        {
            try
            {
                var response = await _eventService.GetEventsByUserId(eventDto, userId);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<EventDto>.ErrorResponse();
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EventDto>>> Create([FromForm] EventDto eventDto, List<int> userIds)
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub); // User ID from the token
                var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email); // User email from the token
                var userRole = User.FindFirstValue(ClaimTypes.Role); // User role from the token

                var response = await _eventService.Create(eventDto, userIds);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<EventDto>.ErrorResponse();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Update(int id, [FromForm] EventDto eventDto, List<int> userIds)
        {
            try
            {
                var response = await _eventService.Update(id, eventDto, userIds);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<EventDto>.ErrorResponse();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Delete(int? id)
        {
            try
            {
                var response = await _eventService.Delete(id);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<EventDto>.ErrorResponse();
            }
        }
    }
}
