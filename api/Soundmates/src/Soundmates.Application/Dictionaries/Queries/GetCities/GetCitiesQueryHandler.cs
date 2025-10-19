using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetCities;

public class GetCitiesQueryHandler(
    IDictionaryRepository _dictionaryRepository    
) : IRequestHandler<GetCitiesQuery, Result<List<CityDto>>>
{
    public async Task<Result<List<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var cities = await _dictionaryRepository.GetAllCountryCitiesAsync(request.CountryId);

        var citiesDtos = cities.Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            CountryId = c.CountryId
        }).ToList();

        return Result<List<CityDto>>.Success(citiesDtos);
    }
}
