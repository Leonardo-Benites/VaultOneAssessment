using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.User.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByIds(List<int> userIds)
        {
            return await _context.User.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToListAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _context.User.AsNoTracking().AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetById(int id)
        {
            return await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Insert(User obj)
        {
            _context.User.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User obj)
        {
            _context.User.Update(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(User obj)
        {
            _context.User.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
