using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Common;

namespace Soundmates.Application.Users.Queries.GetOtherUserProfile;

public record GetOtherUserProfileQuery(int OtherUserId, string? SubClaim) : IRequest<Result<OtherUserProfileDto>>;
