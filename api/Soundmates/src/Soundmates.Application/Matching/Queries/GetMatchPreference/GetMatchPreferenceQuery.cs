using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Matching;

namespace Soundmates.Application.Matching.Queries.GetMatchPreference;

public record GetMatchPreferenceQuery(string? SubClaim) : IRequest<Result<MatchPreferenceDto>>;
