using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Commands.CreateDislike;

public class CreateDislikeCommandHandler(
    IUserRepository _userRepository,
    IMatchRepository _matchRepository,
    IAuthService _authService
) : IRequestHandler<CreateDislikeCommand, Result>
{
    public async Task<Result> Handle(CreateDislikeCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var receiverExists = await _userRepository.CheckIfExistsActiveAsync(request.ReceiverId);
        if (!receiverExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.ReceiverId}");
        }

        var reactionExists = await _matchRepository.CheckIfReactionExistsAsync(authorizedUser.Id, request.ReceiverId);
        if (reactionExists)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Cannot give another reaction to the same user. From: {authorizedUser.Id} To: {request.ReceiverId}");
        }

        var dislike = new Dislike
        {
            GiverId = authorizedUser.Id,
            ReceiverId = request.ReceiverId
        };

        await _matchRepository.AddDislikeAsync(dislike);

        return Result.Success();
    }
}
