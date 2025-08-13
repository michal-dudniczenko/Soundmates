using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Identity;

namespace Soundmates.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user != null)
        {
            return user.ToDomain();
        }

        return null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);

        if (user != null)
        {
            return user.ToDomain();
        }

        return null;
    }

    public async Task<IEnumerable<User>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .Select(e => e.ToDomain())
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .Where(e => e.IsActive)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .Select(e => e.ToDomain())
            .ToListAsync();
    }
}
