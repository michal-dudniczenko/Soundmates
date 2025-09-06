using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(string Name, string Description, int BirthYear, string City, string Country, string? SubClaim) : IRequest<Result>;
