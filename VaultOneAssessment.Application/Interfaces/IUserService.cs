using Application.Dtos;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task<ApiResponse<IEnumerable<UserDto>>> GetAll();
        public Task<ApiResponse<UserDto>> GetById(int id);
        public Task<ApiResponse<UserDto>> Create(UserDto user);
        public Task<ApiResponse<UserDto>> Update(int id, UserDto user);
        public Task<ApiResponse<UserDto>> Delete(int id);
    }
}
