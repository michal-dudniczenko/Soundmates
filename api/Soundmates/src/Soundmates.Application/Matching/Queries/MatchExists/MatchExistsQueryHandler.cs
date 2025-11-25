using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.MatchExists;

public class MatchExistsQueryHandler(
    IMatchRepository _matchRepository,
    IAuthService _authService
) : IRequestHandler<MatchExistsQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(MatchExistsQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<bool>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (authorizedUser.Id == request.OtherUserId)
        {
            return Result<bool>.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "User can't have matched yourself.");
        }

        var matchExists = await _matchRepository.CheckIfMatchExistsAsync(authorizedUser.Id, request.OtherUserId);

        return Result<bool>.Success(matchExists);
    }
}

