using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Common;

namespace Soundmates.Application.Users.Queries.GetSelfUserProfile;

public record GetSelfUserProfileQuery(string? SubClaim) : IRequest<Result<SelfUserProfileDto>>;