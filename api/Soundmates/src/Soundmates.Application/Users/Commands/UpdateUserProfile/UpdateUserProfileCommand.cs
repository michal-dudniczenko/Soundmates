using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile;

public abstract record UpdateUserProfileCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<TagDto> Tags, 
    IList<Guid> MusicSamplesOrder,
    IList<Guid> ProfilePicturesOrder,
    string? SubClaim
) : IRequest<Result>;
