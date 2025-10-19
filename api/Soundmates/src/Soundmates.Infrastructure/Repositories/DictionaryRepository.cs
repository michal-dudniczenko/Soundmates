using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class DictionaryRepository : IDictionaryRepository
{
    public Task<IEnumerable<BandRole>> GetAllBandRolesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<City>> GetAllCountryCitiesAsync(Guid countryId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Gender>> GetAllGendersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TagCategory>> GetAllTagCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Tag> GetTagByIdAsync(Guid tagId)
    {
        throw new NotImplementedException();
    }
}
