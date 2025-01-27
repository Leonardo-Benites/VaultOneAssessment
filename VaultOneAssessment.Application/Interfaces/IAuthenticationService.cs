using Application.Dtos;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<ApiResponse<UserDto>> Login(UserDto dto);
        public string GenerateJwtToken(UserDto user);
    }
}
