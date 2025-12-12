using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetCountries;

public class GetCountriesQueryHandler(
    IDictionaryRepository _dictionaryRepository
) : IRequestHandler<GetCountriesQuery, Result<List<CountryDto>>>
{
    public async Task<Result<List<CountryDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var countries = await _dictionaryRepository.GetAllCountriesAsync();

        var countriesDtos = countries.Select(c => new CountryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return Result<List<CountryDto>>.Success(countriesDtos);
    }
}
