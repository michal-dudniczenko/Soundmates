using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Mappings;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.GetMatches;

public class GetMatchesQueryHandler(
    IUserRepository _userRepository,
    IAuthService _authService,
    IMatchRepository _matchRepository,
    IArtistRepository _artistRepository,
    IBandRepository _bandRepository
) : IRequestHandler<GetMatchesQuery, Result<List<OtherUserProfileDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<OtherUserProfileDto>>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserProfileDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserProfileDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var matches = await _matchRepository.GetUserMatchesAsync(authorizedUser.Id, request.Limit, request.Offset);

        var userProfiles = new List<OtherUserProfileDto>();

        foreach (var match in matches)
        {
            var user = await _userRepository.GetByIdAsync(
                match.User1Id == authorizedUser.Id ? match.User2Id : match.User1Id
            );
            if (user is null || !user.IsActive || user.IsFirstLogin || !user.IsEmailConfirmed || user.IsBand is null) continue;

            if ((bool)user.IsBand)
            {
                var band = await _bandRepository.GetByUserIdAsync(user.Id);
                if (band is null) continue;

                userProfiles.Add(band.ToOtherUserProfileDto());
            }
            else
            {
                var artist = await _artistRepository.GetByUserIdAsync(user.Id);
                if (artist is null) continue;

                userProfiles.Add(artist.ToOtherUserProfileDto());
            }
        }

        return Result<List<OtherUserProfileDto>>.Success(userProfiles);
    }
}
