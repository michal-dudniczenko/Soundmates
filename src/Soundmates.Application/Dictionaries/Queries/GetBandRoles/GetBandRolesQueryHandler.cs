using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetBandRoles;

public class GetBandRolesQueryHandler(
    IDictionaryRepository _dictionaryRepository
) : IRequestHandler<GetBandRolesQuery, Result<List<BandRoleDto>>>
{
    public async Task<Result<List<BandRoleDto>>> Handle(GetBandRolesQuery request, CancellationToken cancellationToken)
    {
        var bandRoles = await _dictionaryRepository.GetAllBandRolesAsync();

        var bandRolesDtos = bandRoles.Select(br => new BandRoleDto
        {
            Id = br.Id,
            Name = br.Name
        }).ToList();

        return Result<List<BandRoleDto>>.Success(bandRolesDtos);
    }
}
