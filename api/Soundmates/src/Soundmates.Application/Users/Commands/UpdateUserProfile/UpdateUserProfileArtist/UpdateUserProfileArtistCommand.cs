using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;

public record UpdateUserProfileArtistCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<Guid> Tags, 
    DateOnly BirthDate, 
    string? SubClaim
) : UpdateUserProfileCommand(Name, Description, CountryId, CityId, Tags, SubClaim), 
    IRequest<Result>;
