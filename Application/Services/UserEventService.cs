using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Repositories;


namespace Application.Services
{
    public class UserEventService : IUserEventService
    {
        private readonly UserEventRepository _userEventRepository;
        private readonly UserRepository _userRepository;

        public UserEventService(AppDbContext context)
        {
            _userEventRepository = new UserEventRepository(context);
            _userRepository = new UserRepository(context);
        }

        public async Task<ApiResponse<List<int>>> GetUserIdsByEventId(int? eventId)
        {
            if (eventId == null || eventId == 0)
            {
                return new ApiResponse<List<int>>
                {
                    Data = null,
                    Message = "Algo deu errado, o ID do evento não foi informado na Requisição.",
                    Code = 400,
                    Success = false
                };
            }

            return new ApiResponse<List<int>>
            {
                Data = await _userEventRepository.GetUserIdsByEventId((int)eventId),
                Message = "Busca de usuários vinculados ao evento com sucesso.",
                Code = 200,
                Success = true
            };
        }

        public async Task<ApiResponse<bool>> SubscribeUsersOnEventCreate(List<int> userIds, int eventId)
        {
            var users = await _userRepository.GetByIds(userIds);
            if (users == null || !users.Any())
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Nenhum usuário válido foi encontrado para associar ao evento.",
                    Code = 404,
                    Success = false
                };
            }

            var userEvents = users.Select(user => new UserEvent
            {
                UserId = user.Id,
                SubscribedDate = DateTime.UtcNow
            }).ToList();

            userEvents.ForEach(userEvent => userEvent.EventId = eventId);

            await _userEventRepository.InsertRange(userEvents);
            await _userEventRepository.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Message = "Usuários associados com sucesso.",
                Code = 200,
                Success = true
            };
        }

        public async Task<ApiResponse<bool>> SubscribeUsersOnEventUpdate(List<int> selectedUserIds, int eventId)
        {
            if (selectedUserIds == null || !selectedUserIds.Any())
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Message = "O Evento precisa de ao menos um usuário inscrito.",
                    Code = 400,
                    Success = false
                };
            }

            var currentUserIds = await _userEventRepository.GetUserIdsByEventId(eventId); //verificar QUANDO FOR NULL se vai dar erro

            var subscribeResult = await HandleSubscribeUsers(currentUserIds, selectedUserIds, eventId);
            if (!subscribeResult)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Erro ao associar usuários ao evento. Tente novamente.",
                    Code = 500,
                    Success = false
                };
            }

            var unsubscribeResult = await HandleUnsubscribeUsers(currentUserIds, selectedUserIds, eventId);
            if (!unsubscribeResult)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Erro ao remover usuários do evento. Tente novamente.",
                    Code = 500,
                    Success = false
                };
            }

            await _userEventRepository.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Message = "Usuários associados com sucesso.",
                Code = 200,
                Success = true
            };
        }

        private async Task<bool> HandleSubscribeUsers(List<int> currentUserIds, List<int> selectedUserIds, int eventId)
        {
            var usersToAdd = new List<int>();

            // Se não há usuários já inscritos, adiciona todos os selecionados
            if (currentUserIds == null || !currentUserIds.Any())
            {
                usersToAdd = selectedUserIds;
            }
            // Se já há usuários inscritos, adiciona apenas os selecionados que não estão na lista atual
            else
            {
                usersToAdd = selectedUserIds.Except(currentUserIds).ToList();
            }

            if (usersToAdd.Any())
            {
                var modelAdd = usersToAdd.Select(userId => new UserEvent
                {
                    UserId = userId,
                    EventId = eventId,
                    SubscribedDate = DateTime.UtcNow
                }).ToList();

                await _userEventRepository.InsertRange(modelAdd);
                return true;  
            }

            return false;  
        }


        private async Task<bool> HandleUnsubscribeUsers(List<int> currentUserIds, List<int> selectedUserIds, int eventId)
        {
            var usersToRemove = new List<int>();

            usersToRemove = currentUserIds.Except(selectedUserIds).ToList();

            if (usersToRemove.Any())
            {
                var modelRemove = usersToRemove.Select(userId => new UserEvent
                {
                    UserId = userId,
                    EventId = eventId,
                    SubscribedDate = DateTime.UtcNow
                }).ToList();

                await _userEventRepository.RemoveRange(modelRemove);
                return true; 
            }

            return false;  
        }

        public async Task<ApiResponse<EventDto>> Subscribe(int? eventId, int userId)
        {
            if (eventId == null || eventId == 0)
            {
                return new ApiResponse<EventDto>
                {
                    Message = $"ID do evento não foi enviado na requisição, atualize a página e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var model = new UserEvent() { EventId = (int)eventId, UserId = userId };

            await _userEventRepository.Insert(model);

            return ApiResponse<EventDto>.SuccessResponse(null, "Inscrição no evento realizada com sucesso", 201);
        }

        public async Task<ApiResponse<EventDto>> Unsubscribe(int? eventId, int userId)
        {
            if (eventId == null || eventId == 0)
            {
                return new ApiResponse<EventDto>
                {
                    Message = $"ID do evento não foi enviado na requisição, atualize a página e tente novamente.",
                    Code = 400,
                    Success = false
                };
            }

            var model = new UserEvent() { EventId = (int)eventId, UserId = userId };

            await _userEventRepository.Remove(model);

            return ApiResponse<EventDto>.SuccessResponse(null, "Desinscrição no evento realizada com sucesso", 201);
        }


    }
}
