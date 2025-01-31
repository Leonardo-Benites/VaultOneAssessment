using Application.Dtos;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IUserEventService
    {
        public Task<ApiResponse<List<int>>> GetUserIdsByEventId(int? eventId);
        public Task<ApiResponse<bool>> SubscribeUsersOnEventCreate(List<int> userIds, int eventId);
        public Task<ApiResponse<bool>> SubscribeUsersOnEventUpdate(List<int> userIds, int eventId);
        public Task<ApiResponse<EventDto>> Subscribe(int? eventId, int userId);
        public Task<ApiResponse<EventDto>> Unsubscribe(int? eventId, int userId);

    }
}
