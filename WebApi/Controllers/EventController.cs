using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserEventService _userEventService;

        public EventController(IEventService eventService, IUserEventService userEventService)
        {
            _eventService = eventService;
            _userEventService = userEventService;
        }

        [HttpGet("GetPublicEvents")]
        public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetPublicEvents([FromQuery] EventDto eventDto)
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
                return ApiResponse<IEnumerable<EventDto>>.ErrorResponse();
            }
        }

        [Authorize]
        [HttpGet("GetMyEventsAndPublic")]
        public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetMyEventsAndPublic([FromQuery] EventDto eventDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault().Value;

                var response = await _eventService.GetEventsByUserId(eventDto, Convert.ToInt32(userId));

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<EventDto>>.ErrorResponse();
            }
        }

        [Authorize]
        [HttpGet("GetUserIdsByEventId/{eventId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetUserIdsByEventId([FromQuery] int? eventId)
        {
            try
            {
                var response = await _userEventService.GetUserIdsByEventId(eventId);

                if (!response.Success)
                {
                    return StatusCode(response.Code, response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<EventDto>>.ErrorResponse();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Create([FromBody] EventDto eventDto)
        {
            try
            {
                var response = await _eventService.Create(eventDto);

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
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Update(int id, [FromBody] EventDto eventDto)
        {
            try
            {
                var response = await _eventService.Update(id, eventDto);

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
        [HttpDelete("Delete/{id}")]
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

        [Authorize]
        [HttpDelete("Subscribe")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Subscribe(int? eventId)
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                var response = await _userEventService.Subscribe(eventId, Convert.ToInt32(userId));

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
        [HttpDelete("Unsubscribe")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Unsubscribe(int? eventId)
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                var response = await _userEventService.Unsubscribe(eventId, Convert.ToInt32(userId));

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
