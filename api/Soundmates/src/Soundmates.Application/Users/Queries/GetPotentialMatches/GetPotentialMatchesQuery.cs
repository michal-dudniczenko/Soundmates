using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Common;

namespace Soundmates.Application.Users.Queries.GetPotentialMatches;

public record GetPotentialMatchesQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<OtherUserProfileDto>>>;
