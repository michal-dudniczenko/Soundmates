using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile;

public abstract record UpdateUserProfileCommand(
    string Name, 
    string Description, 
    Guid CountryId, 
    Guid CityId, 
    IList<Guid> Tags, 
    string? SubClaim
) : IRequest<Result>;
