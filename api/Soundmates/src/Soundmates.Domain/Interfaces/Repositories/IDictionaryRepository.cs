using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDictionaryRepository
{
    Task<IEnumerable<Country>> GetAllCountriesAsync();
    Task<IEnumerable<City>> GetAllCountryCitiesAsync(Guid CountryId);
    Task<IEnumerable<BandRole>> GetAllBandRolesAsync();
    Task<IEnumerable<TagCategory>> GetAllTagCategoriesAsync();
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<IEnumerable<Gender>> GetAllGendersAsync();
}
