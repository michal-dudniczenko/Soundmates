using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetCities;

public record GetCitiesQuery(Guid CountryId) : IRequest<Result<List<CityDto>>>;
