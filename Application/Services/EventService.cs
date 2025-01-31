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
        private readonly UserEventRepository _userEventRepository;
        readonly IMapper _mapper;
        readonly IUserEventService _userEventService;

        public EventService(AppDbContext context, IMapper mapper, IUserEventService userEventService)
        {
            _eventRepository = new EventRepository(context);
            _userEventRepository = new UserEventRepository(context);
            _mapper = mapper;
            _userEventService = userEventService;
        }

        public async Task<ApiResponse<IEnumerable<EventDto>>> GetPublicEvents(EventDto eventDto)
        {
            var events = await _eventRepository.GetPublicEvents(eventDto.KeyWords, eventDto.Date);

            if (events is null || !events.Any())
            {
                return new ApiResponse<IEnumerable<EventDto>>
                {
                    Data = null,
                    Message = "Nenhum evento foi encontrado com os filtros aplicados.",
                    Code = 404,
                    Success = false
                };
            }

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);

            return ApiResponse<EventDto>.SuccessResponseCollection(eventsDto);
        }

        public async Task<ApiResponse<IEnumerable<EventDto>>> GetEventsByUserId(EventDto eventDto, int userId)
        {
            var events = await _eventRepository.GetEventsByUserId(userId, eventDto.KeyWords, eventDto.Date);

            if (events is null || !events.Any())
            {
                return new ApiResponse<IEnumerable<EventDto>>
                {
                    Data = null,
                    Message = "Nenhum evento encontrado para este usuário com os filtros aplicados.",
                    Code = 404,
                    Success = false
                };
            }

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);
            foreach (var dto in eventsDto)
            {
                var userIds = events.FirstOrDefault(e => e.Id == dto.Id)?
                    .UserEvents?.Select(ue => ue.UserId).ToList() ?? new List<int>();

                dto.UserIds = userIds;
            }

            return ApiResponse<EventDto>.SuccessResponseCollection(eventsDto);
        }

        public async Task<ApiResponse<EventDto>> Create(EventDto dto)
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

            if (dto.UserIds != null || dto.UserIds.Any())
            {
                var subscribeResult = await _userEventService.SubscribeUsersOnEventCreate(dto.UserIds, eventModel.Id);
                if (!subscribeResult.Success)
                    warningMessage = subscribeResult.Message;
            }

            string responseMessage = "Evento criado com sucesso." + (warningMessage != null ? $" Aviso: {warningMessage}" : "");

            return ApiResponse<EventDto>.SuccessResponse(null, responseMessage, 201);
        }

        public async Task<ApiResponse<EventDto>> Update(int eventId, EventDto eventDto)
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

            if (eventDto.UserIds != null || eventDto.UserIds.Any())
            {
                var subscribeResult = await _userEventService.SubscribeUsersOnEventUpdate(eventDto.UserIds, model.Id);
                if (!subscribeResult.Success)
                    warningMessage = subscribeResult.Message;
            }

            string responseMessage = "Evento criado com sucesso." + (warningMessage != null ? $" Aviso: {warningMessage}" : "");

            return ApiResponse<EventDto>.SuccessResponse(null, responseMessage, 201);
        }

        public async Task<ApiResponse<EventDto>> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return new ApiResponse<EventDto>
                {
                    Message = $"ID do evento não foi enviado na requisição, atualize a página e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var eventModel = await _eventRepository.GetById((int)id);
            if (eventModel == null)
            {
                return new ApiResponse<EventDto>
                {
                    Message = "Evento não encontrado.",
                    Code = 404,
                    Success = false
                };
            }

            await _userEventRepository.DeleteByEventId((int)id);

            await _eventRepository.Delete(eventModel);

            return ApiResponse<EventDto>.SuccessResponse(null, "Evento deletado com sucesso", 201);
        }

              //TODO: VERIFICAR COMO VALIDAR TYPE
        private bool IsRequiredFieldsFulfilled(EventDto eventDto)
        {
            return !(string.IsNullOrEmpty(eventDto.Name) ||
                string.IsNullOrEmpty(eventDto.Description) ||
                string.IsNullOrEmpty(eventDto.KeyWords));
        }
    }
}
