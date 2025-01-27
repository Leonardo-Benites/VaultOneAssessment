using Infrastructure.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<IEnumerable<Event>> GetPublicEvents()
        {
            return await _context.Event.Where(e => e.Type == Domain.Enums.EventType.Public).ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAll()
        {
            return await _context.Event.ToListAsync();
        }
        public async Task<IEnumerable<Event>> GetPrivateEvents()
        {
            return await _context.Event.Where(e => e.Type == Domain.Enums.EventType.Private).ToListAsync();
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
    }
}
