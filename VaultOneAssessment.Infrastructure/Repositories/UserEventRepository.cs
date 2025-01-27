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

        public async Task Insert(List<UserEvent> userEvents)
        {
            await _context.UserEvent.AddRangeAsync(userEvents);
        }

        public async Task Remove(List<UserEvent> userEvents)
        {
            _context.UserEvent.RemoveRange(userEvents);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
