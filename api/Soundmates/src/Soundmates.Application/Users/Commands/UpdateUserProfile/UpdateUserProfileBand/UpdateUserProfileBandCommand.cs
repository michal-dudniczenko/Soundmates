using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;

public record UpdateUserProfileBandCommand(
    string Name,
    string Description,
    Guid CountryId,
    Guid CityId,
    IList<Guid> Tags,
    IList<BandMemberDto> BandMembers,
    string? SubClaim
) : UpdateUserProfileCommand(Name, Description, CountryId, CityId, Tags, SubClaim),
    IRequest<Result>;