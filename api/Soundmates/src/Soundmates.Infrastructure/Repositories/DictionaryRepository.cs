using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class DictionaryRepository(
    ApplicationDbContext _context
) : IDictionaryRepository
{
    public async Task<IEnumerable<BandRole>> GetAllBandRolesAsync()
    {
        return await _context.BandRoles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
        return await _context.Countries
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<City>> GetAllCountryCitiesAsync(Guid countryId)
    {
        return await _context.Cities
            .AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Gender>> GetAllGendersAsync()
    {
        return await _context.Genders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TagCategory>> GetAllTagCategoriesAsync()
    {
        return await _context.TagCategories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .ToListAsync();
    }
}
