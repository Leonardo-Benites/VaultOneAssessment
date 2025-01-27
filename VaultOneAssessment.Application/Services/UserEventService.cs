using Application.Dtos;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserEventService : IUserEventService
    {
        private readonly UserEventRepository _userEventRepository;
        private readonly UserRepository _userRepository;

        public UserEventService(AppDbContext context)
        {
            _userEventRepository = new UserEventRepository(context);
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

            await _userEventRepository.Insert(userEvents);

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
            var currentUserIds = await _userEventRepository.GetUserIdsByEventId(eventId);

            await HandleSubscribeUsers(currentUserIds, selectedUserIds, eventId);

            await HandleUnsubscribeUsers(currentUserIds, selectedUserIds, eventId);
            
            //salva após as duas transações concluídas
            await _userEventRepository.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Message = "Usuários associados com sucesso.",
                Code = 200,
                Success = true
            };
        }

        private async Task HandleSubscribeUsers(List<int> currentUserIds, List<int> selectedUserIds, int eventId)
        {
            var usersToAdd = new List<int>();

            //Se não tem usuário na base adiciona os selecionados
            if (currentUserIds == null || !currentUserIds.Any())
            {
                usersToAdd = selectedUserIds;
            }
            //se tem usuário na base, difere os atuais dos novos adicionando somente os novos
            else
            {
                usersToAdd = selectedUserIds.Except(currentUserIds).ToList();
            }

            var modelAdd = usersToAdd.Select(userId => new UserEvent
            {
                UserId = userId,
                EventId = eventId,
                SubscribedDate = DateTime.UtcNow
            }).ToList();

            await _userEventRepository.Insert(modelAdd);
        }

        private async Task HandleUnsubscribeUsers(List<int> currentUserIds, List<int> selectedUserIds, int eventId)
        {
            var usersToRemove = new List<int>();

            if (selectedUserIds == null || !selectedUserIds.Any())
            {
                //Adicionar uma validação para retornaru mensagem pro usuario informando que deve ser selecionado ao menos 1 usuario ou evento será encerrado.
            }
            else
            {
                usersToRemove = currentUserIds.Except(selectedUserIds).ToList();
            }

            var modelRemove = usersToRemove.Select(userId => new UserEvent
            {
                UserId = userId,
                EventId = eventId,
                SubscribedDate = DateTime.UtcNow
            }).ToList();

            await _userEventRepository.Remove(modelRemove);
        }

    }
}
