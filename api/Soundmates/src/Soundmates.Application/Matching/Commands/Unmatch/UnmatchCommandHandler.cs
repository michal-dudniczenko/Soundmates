using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Commands.Unmatch;

public class UnmatchCommandHandler(
    IMatchRepository _matchRepository,
    IAuthService _authService
) : IRequestHandler<UnmatchCommand, Result>
{
    public async Task<Result> Handle(UnmatchCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (request.OtherUserId == authorizedUser.Id)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "You cannot unmatch yourself.");
        }

        var matchExists = await _matchRepository.CheckIfMatchExistsAsync(authorizedUser.Id, request.OtherUserId);

        if (!matchExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"Match with user : {request.OtherUserId} does not exist.");
        }

        await _matchRepository.DeleteMatchAsync(authorizedUser.Id, request.OtherUserId);

        return Result.Success();
    }
}
