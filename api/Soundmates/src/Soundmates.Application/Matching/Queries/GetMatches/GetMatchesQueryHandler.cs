using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.GetMatches;

public class GetMatchesQueryHandler(
    IUserRepository _userRepository,
    IAuthService _authService,
    IMatchRepository _matchRepository
) : IRequestHandler<GetMatchesQuery, Result<List<OtherUserProfileDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<OtherUserProfileDto>>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<MatchUserProfile>>(request.Limit, request.Offset, MaxLimit);
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
            if (user is null || !user.IsActive || user.IsFirstLogin || !user.IsEmailConfirmed) continue;

            userProfiles.Add(new OtherUserProfileDto
            {
                Id = user.Id,
                Name = user.Name!,
                Description = user.Description!,
                BirthYear = (int)user.BirthYear!,
                City = user.City!,
                Country = user.Country!
            });
        }

        return Result<List<OtherUserProfileDto>>.Success(userProfiles);
    }
}
