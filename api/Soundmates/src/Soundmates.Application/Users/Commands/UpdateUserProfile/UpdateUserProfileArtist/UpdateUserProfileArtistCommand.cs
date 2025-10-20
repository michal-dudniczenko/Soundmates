using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;

public record UpdateUserProfileArtistCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<TagDto> Tags,
    IList<Guid> MusicSamplesOrder,
    IList<Guid> ProfilePicturesOrder,
    DateOnly BirthDate,
    Guid GenderId,
    string? SubClaim
) : UpdateUserProfileCommand(Name, Description, CountryId, CityId, Tags, MusicSamplesOrder, ProfilePicturesOrder, SubClaim), 
    IRequest<Result>;
