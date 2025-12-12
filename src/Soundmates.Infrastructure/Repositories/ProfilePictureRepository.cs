using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class ProfilePictureRepository(
    ApplicationDbContext _context
) : IProfilePictureRepository
{
    public async Task AddAsync(ProfilePicture entity)
    {
        _context.ProfilePictures.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<ProfilePicture?> GetByIdAsync(Guid entityId)
    {
        return await _context.ProfilePictures
            .AsNoTracking()
            .FirstOrDefaultAsync(pp => pp.Id == entityId);
    }

    public async Task<int> GetUserProfilePicturesCountAsync(Guid userId)
    {
        return await _context.ProfilePictures
            .AsNoTracking()
            .Where(pp => pp.UserId == userId)
            .CountAsync();
    }

    public async Task RemoveAsync(Guid entityId)
    {
        var picture = await _context.ProfilePictures
            .FindAsync(entityId)
            ?? throw new InvalidOperationException($"Picture with id: {entityId} was not found.");

        _context.ProfilePictures.Remove(picture);

        await _context.SaveChangesAsync();
    }
}
