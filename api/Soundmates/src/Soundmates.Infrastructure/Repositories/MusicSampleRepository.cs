using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MusicSampleRepository : IMusicSampleRepository
{
    private readonly ApplicationDbContext _context;

    public MusicSampleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MusicSample?> GetByIdAsync(int entityId)
    {
        return await _context.MusicSamples
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<MusicSample>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.MusicSamples
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(MusicSample entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var currentCount = await GetUserMusicSamplesCountAsync(entity.UserId);

        entity.DisplayOrder = currentCount;

        await _context.MusicSamples.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.MusicSamples.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<MusicSample>(entityId: entityId));

        await _context.MusicSamples
            .Where(e => e.UserId == entity.UserId && e.DisplayOrder > entity.DisplayOrder)
            .ForEachAsync(e => e.DisplayOrder--);

        _context.MusicSamples.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MusicSample entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.MusicSamples.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<MusicSample>(entityId: entity.Id));
        }

        _context.MusicSamples.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.MusicSamples
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DisplayOrder)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUserMusicSamplesCountAsync(int userId)
    {
        return await _context.MusicSamples.CountAsync(e => e.UserId == userId);
    }

    public async Task MoveDisplayOrderUpAsync(int musicSampleId)
    {
        var entity = await _context.MusicSamples.FindAsync(musicSampleId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<MusicSample>(entityId: musicSampleId));

        var nextOrderEntity = await _context.MusicSamples.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder + 1)
            ?? throw new InvalidOperationException("Music sample has already last display order.");

        entity.DisplayOrder++;
        nextOrderEntity.DisplayOrder--;
        await _context.SaveChangesAsync();
    }

    public async Task MoveDisplayOrderDownAsync(int musicSampleId)
    {
        var entity = await _context.MusicSamples.FindAsync(musicSampleId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<MusicSample>(entityId: musicSampleId));

        if (entity.DisplayOrder == 0)
        {
            throw new InvalidOperationException("Music sample is already first in display order.");
        }

        var previousOrderEntity = await _context.MusicSamples.FirstOrDefaultAsync(e => e.UserId == entity.UserId && e.DisplayOrder == entity.DisplayOrder - 1)
            ?? throw new InvalidOperationException("Music sample is already first in display order.");

        entity.DisplayOrder--;
        previousOrderEntity.DisplayOrder++;
        await _context.SaveChangesAsync();
    }
}
