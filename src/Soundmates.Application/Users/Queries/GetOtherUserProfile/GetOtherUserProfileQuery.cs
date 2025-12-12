using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Users.Queries.GetOtherUserProfile;

public record GetOtherUserProfileQuery(Guid OtherUserId, string? SubClaim) : IRequest<Result<OtherUserProfileDto>>;
