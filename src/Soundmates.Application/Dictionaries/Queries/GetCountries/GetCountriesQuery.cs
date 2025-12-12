using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetCountries;

public record GetCountriesQuery(): IRequest<Result<List<CountryDto>>>;
