using Application.Dtos;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IUserEventService
    {
        public Task<ApiResponse<bool>> SubscribeUsersOnEventCreate(List<int> userIds, int eventId);
        public Task<ApiResponse<bool>> SubscribeUsersOnEventUpdate(List<int> userIds, int eventId);
    }
}
