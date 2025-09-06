using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Exceptions.MusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class ProfilePictureRepository : IProfilePictureRepository
{
    private readonly ApplicationDbContext _context;

    public ProfilePictureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProfilePicture?> GetByIdAsync(Guid entityId)
    {
        return await _context.ProfilePictures
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<ProfilePicture>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.ProfilePictures
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        return await _context.ProfilePictures.AnyAsync(e => e.Id == entityId);
    }

    public async Task<Guid> AddAsync(ProfilePicture entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var currentCount = await GetUserProfilePicturesCountAsync(entity.UserId);

        entity.DisplayOrder = currentCount;

        _context.ProfilePictures.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(ProfilePicture entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.ProfilePictures.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(Guid entityId)
    {
        var entity = await _context.ProfilePictures.FindAsync(entityId);

        if (entity is null) return false;

        await _context.ProfilePictures
            .Where(e => e.UserId == entity.UserId && e.DisplayOrder > entity.DisplayOrder)
            .ForEachAsync(e => e.DisplayOrder--);

        _context.ProfilePictures.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ProfilePicture>> GetUserProfilePicturesAsync(Guid userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.ProfilePictures
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DisplayOrder)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUserProfilePicturesCountAsync(Guid userId)
    {
        return await _context.ProfilePictures.CountAsync(e => e.UserId == userId);
    }

    public async Task<bool> MoveDisplayOrderUpAsync(Guid pictureId)
    {
        var entity = await _context.ProfilePictures.FindAsync(pictureId);

        if (entity is null) return false;

        var nextOrderEntity = await _context.ProfilePictures.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder + 1)
            ?? throw new DisplayOrderAlreadyLastException();

        entity.DisplayOrder++;
        nextOrderEntity.DisplayOrder--;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MoveDisplayOrderDownAsync(Guid pictureId)
    {
        var entity = await _context.ProfilePictures.FindAsync(pictureId);

        if (entity is null) return false;

        if (entity.DisplayOrder == 0)
        {
            throw new DisplayOrderAlreadyLastException();
        }

        var previousOrderEntity = await _context.ProfilePictures.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder - 1)
            ?? throw new DisplayOrderAlreadyLastException();

        entity.DisplayOrder--;
        previousOrderEntity.DisplayOrder++;
        await _context.SaveChangesAsync();

        return true;
    }
}
