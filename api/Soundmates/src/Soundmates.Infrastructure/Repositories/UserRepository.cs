using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfEmailExistsAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfExistsActiveAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash)
    {
        throw new NotImplementedException();
    }

    public Task DeactivateUserAccountAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task LogInUserAsync(Guid userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt)
    {
        throw new NotImplementedException();
    }

    public Task LogOutUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserPasswordAsync(Guid userId, string newPasswordHash)
    {
        throw new NotImplementedException();
    }
}
