using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Queries.MatchExists;

public record MatchExistsQuery(Guid OtherUserId, string? SubClaim) : IRequest<Result<bool>>;
