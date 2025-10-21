using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;

public record UpdateUserProfileArtistCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<Guid> TagsIds,
    IList<Guid> MusicSamplesOrder,
    IList<Guid> ProfilePicturesOrder,
    DateOnly BirthDate,
    Guid GenderId,
    string? SubClaim
) : UpdateUserProfileCommand(Name, Description, CountryId, CityId, TagsIds, MusicSamplesOrder, ProfilePicturesOrder, SubClaim), 
    IRequest<Result>;
