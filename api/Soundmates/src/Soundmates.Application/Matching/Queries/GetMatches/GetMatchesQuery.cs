using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Queries.GetMatches;

public record GetMatchesQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<MatchUserProfile>>>;