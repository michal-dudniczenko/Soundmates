using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile;

public abstract record UpdateUserProfileCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<Guid> TagsIds, 
    IList<Guid> MusicSamplesOrder,
    IList<Guid> ProfilePicturesOrder,
    string? SubClaim
) : IRequest<Result>;
