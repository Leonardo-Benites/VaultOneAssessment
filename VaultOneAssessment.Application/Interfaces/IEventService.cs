using Application.Dtos;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IEventService
    {
        public Task<ApiResponse<IEnumerable<EventDto>>> GetPublicEvents();
        public Task<ApiResponse<IEnumerable<EventDto>>> GetAll();
        public Task<ApiResponse<EventDto>> Create(EventDto eventDto, List<int> userIds);
        public Task<ApiResponse<EventDto>> Update(int eventId, EventDto eventDto, List<int> userIds);
    }
}
