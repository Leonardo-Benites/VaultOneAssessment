using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResponse<EventDto>>> GetPublicEvents()
        {
            try
            {
                var response = await _eventService.GetPublicEvents();

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

        [HttpGet(Name = "GetAllEvents")]
        public async Task<ActionResult<ApiResponse<EventDto>>> GetAll()
        {
            try
            {
                var response = await _eventService.GetAll();

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
        [HttpPost(Name = "Create")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Create([FromForm] EventDto eventDto, List<int> userIds)
        {
            try
            {
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

        [Authorize]
        [HttpPost(Name = "Update")]
        public async Task<ActionResult<ApiResponse<EventDto>>> Update([FromForm] EventDto eventDto, int id, List<int> userIds)
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
    }
}
