using Application.Interfaces;
using Application.Dtos;
using Application.Responses;
using Infrastructure.Context;
using Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserRepository _userRepository;
        readonly IMapper _mapper;
        readonly IConfiguration _configuration;

        public AuthenticationService(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<UserDto>> Login(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Email))
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Informe o email para realizar o login.",
                    Code = 400,
                    Success = false
                };
            }

            if (string.IsNullOrEmpty(userDto.Password))
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Informe a senha para realizar o login.",
                    Code = 400,
                    Success = false
                };
            }

            var user = await _userRepository.GetByEmail(userDto.Email);

            if (user is null)
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Não foi possível encontrar seu usuário, verifique o e-mail informado.",
                    Code = 404,
                    Success = false
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Usuário ou senha inválidos, verifique os dados informados e tente novamente.",
                    Code = 401,
                    Success = false
                };
            }

            var token = GenerateJwtToken(userDto);

            return ApiResponse<UserDto>.SuccessResponse(userDto, token);
        }

        public string GenerateJwtToken(UserDto user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Profile),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
