using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Infrastructure.Context;
using Infrastructure.Repositories;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class EventService : IEventService
    {
        private readonly EventRepository _eventRepository;
        private readonly UserRepository _userRepository;
        readonly IMapper _mapper;
        readonly IUserEventService _userEventService;

        public EventService(AppDbContext context, IMapper mapper, IUserEventService userEventService)
        {
            _eventRepository = new EventRepository(context);
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _userEventService = userEventService;
        }

        public async Task<ApiResponse<IEnumerable<EventDto>>> GetPublicEvents()
        {
            var events = await _eventRepository.GetPublicEvents();

            if (events is null)
            {
                return new ApiResponse<IEnumerable<EventDto>>
                {
                    Data = null,
                    Message = $"Nenhum evento foi encontrado",
                    Code = 404,
                    Success = false
                };
            }

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);

            return ApiResponse<EventDto>.SuccessResponseCollection(eventsDto);
        }

        public async Task<ApiResponse<IEnumerable<EventDto>>> GetAll()
        {
            var events = await _eventRepository.GetAll();

            if (events is null)
            {
                return new ApiResponse<IEnumerable<EventDto>>
                {
                    Data = null,
                    Message = $"Nenhum evento foi encontrado",
                    Code = 404,
                    Success = false
                };
            }

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);

            return ApiResponse<EventDto>.SuccessResponseCollection(eventsDto);
        }

        public async Task<ApiResponse<EventDto>> Create(EventDto dto, List<int> userIds)
        {
            if (!IsRequiredFieldsFulfilled(dto))
            {
                return new ApiResponse<EventDto>
                {
                    Data = null,
                    Message = "Verifique os dados enviados e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var eventModel = _mapper.Map<Event>(dto);
            await _eventRepository.Insert(eventModel);

            string warningMessage = "";

            if (userIds != null || userIds.Any())
            {
                var subscribeResult = await _userEventService.SubscribeUsersOnEventCreate(userIds, eventModel.Id);
                if (!subscribeResult.Success)
                    warningMessage = subscribeResult.Message;
            }

            string responseMessage = "Evento criado com sucesso." + (warningMessage != null ? $" Aviso: {warningMessage}" : "");

            return ApiResponse<EventDto>.SuccessResponse(null, responseMessage, 201);
        }

        public async Task<ApiResponse<EventDto>> Update(int eventId, EventDto eventDto, List<int> userIds)
        {
            if (eventId != eventDto.Id)
            {
                return new ApiResponse<EventDto>
                {
                    Message = $"ID do evento não bate, atualize a página e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var model = await _eventRepository.GetById(eventId);
            if (model == null)
            {
                return new ApiResponse<EventDto>
                {
                    Message = "Evento não encontrado.",
                    Code = 404,
                    Success = false
                };
            }

            model = _mapper.Map<Event>(eventDto);
            await _eventRepository.Update(model);

            string warningMessage = "";

            if (userIds != null || userIds.Any())
            {
                var subscribeResult = await _userEventService.SubscribeUsersOnEventUpdate(userIds, model.Id);
                if (!subscribeResult.Success)
                    warningMessage = subscribeResult.Message;
            }

            string responseMessage = "Evento criado com sucesso." + (warningMessage != null ? $" Aviso: {warningMessage}" : "");

            return ApiResponse<EventDto>.SuccessResponse(null, responseMessage, 201);
        }

        private bool IsRequiredFieldsFulfilled(EventDto eventDto)
        {
            return !(string.IsNullOrEmpty(eventDto.Name) ||
                string.IsNullOrEmpty(eventDto.Type) ||
                string.IsNullOrEmpty(eventDto.Description));
        }
    }
}
