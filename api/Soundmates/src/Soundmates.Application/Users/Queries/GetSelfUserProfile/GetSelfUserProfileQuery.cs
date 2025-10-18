using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Users.Queries.GetSelfUserProfile;

public record GetSelfUserProfileQuery(string? SubClaim) : IRequest<Result<SelfUserProfileDto>>;