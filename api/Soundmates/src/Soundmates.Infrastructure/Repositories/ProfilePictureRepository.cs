using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class ProfilePictureRepository : IProfilePictureRepository
{
    private readonly ApplicationDbContext _context;

    public ProfilePictureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProfilePicture?> GetByIdAsync(int entityId)
    {
        return await _context.ProfilePictures
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<ProfilePicture>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.ProfilePictures
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(ProfilePicture entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var currentCount = await GetUserProfilePicturesCountAsync(entity.UserId);

        entity.DisplayOrder = currentCount;

        await _context.ProfilePictures.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.ProfilePictures.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<ProfilePicture>(entityId: entityId));

        await _context.ProfilePictures
            .Where(e => e.UserId == entity.UserId && e.DisplayOrder > entity.DisplayOrder)
            .ForEachAsync(e => e.DisplayOrder--);

        _context.ProfilePictures.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProfilePicture entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.ProfilePictures.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<ProfilePicture>(entityId: entity.Id));
        }

        _context.ProfilePictures.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProfilePicture>> GetUserProfilePicturesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.ProfilePictures
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DisplayOrder)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUserProfilePicturesCountAsync(int userId)
    {
        return await _context.ProfilePictures.CountAsync(e => e.UserId == userId);
    }

    public async Task MoveDisplayOrderUpAsync(int pictureId)
    {
        var entity = await _context.ProfilePictures.FindAsync(pictureId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<ProfilePicture>(entityId: pictureId));

        var nextOrderEntity = await _context.ProfilePictures.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder + 1)
            ?? throw new InvalidOperationException("Picture has already last display order.");

        entity.DisplayOrder++;
        nextOrderEntity.DisplayOrder--;
        await _context.SaveChangesAsync();
    }

    public async Task MoveDisplayOrderDownAsync(int pictureId)
    {
        var entity = await _context.ProfilePictures.FindAsync(pictureId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<ProfilePicture>(entityId: pictureId));

        if (entity.DisplayOrder == 0)
        {
            throw new InvalidOperationException("Picture is already first in display order.");
        }

        var previousOrderEntity = await _context.ProfilePictures.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder - 1)
            ?? throw new InvalidOperationException("Picture is already first in display order.");

        entity.DisplayOrder--;
        previousOrderEntity.DisplayOrder++;
        await _context.SaveChangesAsync();
    }
}
