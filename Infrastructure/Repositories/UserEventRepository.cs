using Infrastructure.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserEventRepository
    {
        private readonly AppDbContext _context;

        public UserEventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetUserIdsByEventId(int eventId)
        {
            return await _context.UserEvent.Select(ue => ue.UserId).ToListAsync();
        }

        public async Task Insert(UserEvent userEvent)
        {
            await _context.UserEvent.AddAsync(userEvent);
            await _context.SaveChangesAsync();

        }
        public async Task Remove(UserEvent userEvent)
        {
            _context.UserEvent.Remove(userEvent);
            await _context.SaveChangesAsync();
        }

        public async Task InsertRange(List<UserEvent> userEvents)
        {
            await _context.UserEvent.AddRangeAsync(userEvents);
        }

        public async Task RemoveRange(List<UserEvent> userEvents)
        {
            _context.UserEvent.RemoveRange(userEvents);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByUserId(int userId)
        {
            var userEvents = await _context.UserEvent.Where(ue => ue.UserId == userId).ToListAsync();
            _context.UserEvent.RemoveRange(userEvents);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByEventId(int eventId)
        {
            var eventUsers = await _context.UserEvent.Where(ue => ue.EventId == eventId).ToListAsync();
            _context.UserEvent.RemoveRange(eventUsers);
            await _context.SaveChangesAsync();
        }
    }
}
