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

        public async Task<ApiResponse<AuthDto>> Login(AuthDto authDto)
        {
            if (string.IsNullOrEmpty(authDto.Email))
            {
                return new ApiResponse<AuthDto>
                {
                    Data = null,
                    Message = $"Informe o email para realizar o login.",
                    Code = 400,
                    Success = false
                };
            }

            if (string.IsNullOrEmpty(authDto.Password))
            {
                return new ApiResponse<AuthDto>
                {
                    Data = null,
                    Message = $"Informe a senha para realizar o login.",
                    Code = 400,
                    Success = false
                };
            }

            var user = await _userRepository.GetByEmail(authDto.Email);

            if (user is null)
            {
                return new ApiResponse<AuthDto>
                {
                    Data = null,
                    Message = $"Não foi possível encontrar seu usuário, verifique o e-mail informado.",
                    Code = 404,
                    Success = false
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(authDto.Password, user.Password))
            {
                return new ApiResponse<AuthDto>
                {
                    Data = null,
                    Message = $"Usuário ou senha inválidos, verifique os dados informados e tente novamente.",
                    Code = 401,
                    Success = false
                };
            }

            var userDto = _mapper.Map<UserDto>(user);

            var token = GenerateJwtToken(userDto);

            return ApiResponse<AuthDto>.SuccessResponse(authDto, "Autenticação realizada com sucesso.", 201,  token);
        }

        public string GenerateJwtToken(UserDto user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Profile.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authenticationKey = Environment.GetEnvironmentVariable("AUTH_KEY");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationKey));
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
