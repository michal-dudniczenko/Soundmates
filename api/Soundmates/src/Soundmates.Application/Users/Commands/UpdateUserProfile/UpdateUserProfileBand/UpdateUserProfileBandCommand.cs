using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;

public record UpdateUserProfileBandCommand(
    string Name,
    string Description,
    Guid CountryId,
    Guid CityId,
    IList<TagDto> Tags,
    IList<Guid> MusicSamplesOrder,
    IList<Guid> ProfilePicturesOrder,
    IList<BandMemberDto> BandMembers,
    string? SubClaim
) : UpdateUserProfileCommand(Name, Description, CountryId, CityId, Tags, MusicSamplesOrder, ProfilePicturesOrder, SubClaim),
    IRequest<Result>;