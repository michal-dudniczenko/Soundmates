using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Exceptions.MusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class MusicSampleRepository : IMusicSampleRepository
{
    private readonly ApplicationDbContext _context;

    public MusicSampleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MusicSample?> GetByIdAsync(Guid entityId)
    {
        return await _context.MusicSamples
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<MusicSample>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.MusicSamples
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        return await _context.MusicSamples.AnyAsync(e => e.Id == entityId);
    }

    public async Task<Guid> AddAsync(MusicSample entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var currentCount = await GetUserMusicSamplesCountAsync(entity.UserId);

        entity.DisplayOrder = currentCount;

        _context.MusicSamples.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(MusicSample entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.MusicSamples.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(Guid entityId)
    {
        var entity = await _context.MusicSamples.FindAsync(entityId);

        if (entity is null) return false;

        await _context.MusicSamples
            .Where(e => e.UserId == entity.UserId && e.DisplayOrder > entity.DisplayOrder)
            .ForEachAsync(e => e.DisplayOrder--);

        _context.MusicSamples.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(Guid userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.MusicSamples
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DisplayOrder)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUserMusicSamplesCountAsync(Guid userId)
    {
        return await _context.MusicSamples.CountAsync(e => e.UserId == userId);
    }

    public async Task<bool> MoveDisplayOrderUpAsync(Guid musicSampleId)
    {
        var entity = await _context.MusicSamples.FindAsync(musicSampleId);

        if (entity is null) return false;

        var nextOrderEntity = await _context.MusicSamples.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder + 1)
            ?? throw new DisplayOrderAlreadyLastException();

        entity.DisplayOrder++;
        nextOrderEntity.DisplayOrder--;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MoveDisplayOrderDownAsync(Guid musicSampleId)
    {
        var entity = await _context.MusicSamples.FindAsync(musicSampleId);

        if (entity is null) return false;

        if (entity.DisplayOrder == 0)
        {
            throw new DisplayOrderAlreadyFirstException();
        }

        var previousOrderEntity = await _context.MusicSamples.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder - 1)
            ?? throw new DisplayOrderAlreadyFirstException();

        entity.DisplayOrder--;
        previousOrderEntity.DisplayOrder++;
        await _context.SaveChangesAsync();

        return true;
    }
}
