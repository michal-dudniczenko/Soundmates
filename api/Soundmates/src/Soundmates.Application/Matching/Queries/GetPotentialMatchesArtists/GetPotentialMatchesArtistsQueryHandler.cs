using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Mappings;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.GetPotentialMatchesArtists;

public class GetPotentialMatchesArtistsQueryHandler(
    IArtistRepository _artistRepository,
    IAuthService _authService
) : IRequestHandler<GetPotentialMatchesArtistsQuery, Result<List<OtherUserProfileArtistDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<OtherUserProfileArtistDto>>> Handle(GetPotentialMatchesArtistsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserProfileArtistDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserProfileArtistDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var artists = await _artistRepository.GetPotentialMatchesAsync(authorizedUser.Id, request.Limit, request.Offset);

        var artistsDtos = artists.Select(a => a.ToOtherUserProfileDto()).ToList();

        return Result<List<OtherUserProfileArtistDto>>.Success(artistsDtos);
    }
}
