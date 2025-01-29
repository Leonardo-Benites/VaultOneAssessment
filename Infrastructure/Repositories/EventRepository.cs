using Infrastructure.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> GetById(int eventId)
        {
            return await _context.Event.FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<IEnumerable<Event>> GetPublicEvents(string? keyword = null, DateTime? date = null)
        {
            var query = _context.Event.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.KeyWords.Contains(keyword));
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            return await query.Where(e => e.Type == Domain.Enums.EventType.Public).ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByUserId(int userId, string? keyword = null, DateTime? date = null)
        {
            var query = _context.Event.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.KeyWords.Contains(keyword));
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            return await query
                .Where(e => e.UserEvents.Any(ue => ue.UserId == userId))
                .ToListAsync();
        }

        public async Task Insert(Event obj)
        {
            _context.Event.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Event obj)
        {
            _context.Event.Update(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Event obj)
        {
            _context.Event.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
