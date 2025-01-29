using Application.Dtos;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IEventService
    {
        public Task<ApiResponse<IEnumerable<EventDto>>> GetPublicEvents(EventDto eventDto);
        public Task<ApiResponse<IEnumerable<EventDto>>> GetEventsByUserId(EventDto eventDto, int userId);
        public Task<ApiResponse<EventDto>> Create(EventDto eventDto, List<int> userIds);
        public Task<ApiResponse<EventDto>> Update(int eventId, EventDto eventDto, List<int> userIds);
        public Task<ApiResponse<EventDto>> Delete(int? id);
    }
}
