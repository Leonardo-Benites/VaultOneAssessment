using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class MappingProfileService : Profile
    {
        public MappingProfileService()
        {
            CreateMap<User, UserDto>()
                .ReverseMap();

            CreateMap<Event, EventDto>()
                .ReverseMap();
        }
    }
}
