using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Matching.Queries.GetPotentialMatchesBands;

public record GetPotentialMatchesBandsQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<OtherUserProfileBandDto>>>;
