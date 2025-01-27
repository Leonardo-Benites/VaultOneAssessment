using Application.Interfaces;
using Domain.Models;
using Application.Dtos;
using Application.Responses;
using Infrastructure.Context;
using Infrastructure.Repositories;
using AutoMapper;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userRepository.GetAll();

            if (users is null)
            {
                return new ApiResponse<IEnumerable<UserDto>>
                {
                    Data = null,
                    Message = $"Nenhum usuario foi encontrado",
                    Code = 404,
                    Success = false
                };
            }

            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

            return ApiResponse<UserDto>.SuccessResponseCollection(usersDto);
        }

        public async Task<ApiResponse<UserDto>> GetById(int id)
        {
            if (id == 0)
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Id não pode ser nulo.",
                    Code = 400,
                    Success = false
                };
            }

            var user = await _userRepository.GetById(id);

            if (user is null)
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = $"Usuario com ID {id} não foi encontrado.",
                    Code = 404,
                    Success = false
                };
            }

            var userDto = _mapper.Map<UserDto>(user);

            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }

        public async Task<ApiResponse<UserDto>> Create(UserDto dto)
        {
            if (!IsRequiredFieldsFulfilled(dto))
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = "Verifique os dados enviados e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            if (await _userRepository.EmailExists(dto.Email))
            {
                return new ApiResponse<UserDto>
                {
                    Data = null,
                    Message = "Este e-mail já está associado a uma conta existente.",
                    Code = 409,
                    Success = false
                };
            }

            dto.Password = GenerateHashPassword(dto.Password);

            var user = _mapper.Map<User>(dto);

            await _userRepository.Insert(user);

            return ApiResponse<UserDto>.SuccessResponse(null, "Usuario criado com sucesso", 201);
        }
        public async Task<ApiResponse<UserDto>> Update(int id, UserDto dto)
        {
            if (id != dto.Id)
            {
                return new ApiResponse<UserDto>
                {
                    Message = $"ID do usuário não bate, atualize a página e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var model = await _userRepository.GetById(id);
            if (model == null)
            {
                return new ApiResponse<UserDto>
                {
                    Message = "Usuario não encontrado.",
                    Code = 404,
                    Success = false
                };
            }

            model = _mapper.Map<User>(dto);

            await _userRepository.Update(model);

            return ApiResponse<UserDto>.SuccessResponse(null, "Usuario atualizado com sucesso");
        }
        public async Task<ApiResponse<UserDto>> Delete(int id)
        {
            var user = await _userRepository.GetById(id);

            await _userRepository.Delete(user);

            return ApiResponse<UserDto>.SuccessResponse(null, "Usuario deletado com sucesso");
        }

        private bool IsRequiredFieldsFulfilled(UserDto UserDto)
        {
            return !(string.IsNullOrEmpty(UserDto.Name) ||
                string.IsNullOrEmpty(UserDto.Email) ||
                string.IsNullOrEmpty(UserDto.Password) ||
                string.IsNullOrEmpty(UserDto.Profile));
        }

        private string GenerateHashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
