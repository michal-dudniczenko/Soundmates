using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetGenders;

public class GetGendersQueryHandler(
    IDictionaryRepository _dictionaryRepository
) : IRequestHandler<GetGendersQuery, Result<List<GenderDto>>>
{
    public async Task<Result<List<GenderDto>>> Handle(GetGendersQuery request, CancellationToken cancellationToken)
    {
        var genders = await _dictionaryRepository.GetAllGendersAsync();

        var gendersDtos = genders.Select(g => new GenderDto
        {
            Id = g.Id,
            Name = g.Name
        }).ToList();

        return Result<List<GenderDto>>.Success(gendersDtos);
    }
}
