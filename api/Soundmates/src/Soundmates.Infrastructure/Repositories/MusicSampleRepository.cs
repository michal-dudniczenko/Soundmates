using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MusicSampleRepository(
    ApplicationDbContext _context
) : IMusicSampleRepository
{
    public async Task AddAsync(MusicSample entity)
    {
        _context.MusicSamples.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<MusicSample?> GetByIdAsync(Guid entityId)
    {
        return await _context.MusicSamples
            .AsNoTracking()
            .FirstOrDefaultAsync(ms => ms.Id == entityId);
    }

    public async Task RemoveAsync(Guid entityId)
    {
        var sample = await _context.MusicSamples
            .FindAsync(entityId)
            ?? throw new InvalidOperationException($"Sample with id: {entityId} was not found.");

        _context.MusicSamples.Remove(sample);

        await _context.SaveChangesAsync();
    }
}
