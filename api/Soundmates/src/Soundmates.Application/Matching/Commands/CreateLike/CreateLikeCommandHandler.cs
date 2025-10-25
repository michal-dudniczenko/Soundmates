using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Commands.CreateLike;

public class CreateLikeCommandHandler(
    IUserRepository _userRepository,
    IMatchRepository _matchRepository,
    IAuthService _authService
) : IRequestHandler<CreateLikeCommand, Result>
{
    public async Task<Result> Handle(CreateLikeCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (request.ReceiverId == authorizedUser.Id)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "You cannot like your own profile.");
        }

        var receiverExists = await _userRepository.CheckIfExistsActiveAsync(request.ReceiverId);
        if (!receiverExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.ReceiverId}");
        }

        var reactionExists = (await _matchRepository.CheckIfLikeExistsAsync(authorizedUser.Id, request.ReceiverId)) || (await _matchRepository.CheckIfDislikeExistsAsync(authorizedUser.Id, request.ReceiverId));
        if (reactionExists)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Cannot give another reaction to the same user. From: {authorizedUser.Id} To: {request.ReceiverId}");
        }

        var like = new Like
        { 
            GiverId = authorizedUser.Id, 
            ReceiverId = request.ReceiverId 
        };

        await _matchRepository.AddLikeAsync(like);

        if (await _matchRepository.CheckIfLikeExistsAsync(giverId: request.ReceiverId, receiverId: authorizedUser.Id))
        {
            var match = new Match
            { 
                User1Id = authorizedUser.Id, 
                User2Id = request.ReceiverId 
            };

            await _matchRepository.AddMatchAsync(match);
        }

        return Result.Success();
    }
}
