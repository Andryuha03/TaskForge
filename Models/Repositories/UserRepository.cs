using Microsoft.EntityFrameworkCore;
using TaskForge.Models.Entities;
namespace TaskForge.Models.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string login);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string login)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Name == login);
        }
    }

}