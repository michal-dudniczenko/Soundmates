using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;

namespace Soundmates.Application.Matching.Queries.GetPotentialMatchesArtists;

public record GetPotentialMatchesArtistsQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<OtherUserProfileArtistDto>>>;
