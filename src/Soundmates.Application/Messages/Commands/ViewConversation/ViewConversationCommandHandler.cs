using MediatR;
using Microsoft.AspNetCore.SignalR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using Soundmates.Infrastructure.SignalRHub;

namespace Soundmates.Application.Messages.Commands.ViewConversation;

public class ViewConversationCommandHandler(
    IUserRepository _userRepository,
    IMessageRepository _messageRepository,
    IMatchRepository _matchRepository,
    IAuthService _authService,
    IHubContext<EventHub> _hubContext
) : IRequestHandler<ViewConversationCommand, Result>
{
    public async Task<Result> Handle(ViewConversationCommand request, CancellationToken cancellationToken)
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
                errorMessage: "You cannot read your own conversation.");
        }

        var otherUserExists = await _userRepository.CheckIfExistsActiveAsync(request.OtherUserId);

        if (!otherUserExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.OtherUserId}");
        }

        var doesMatchExists = await _matchRepository.CheckIfMatchExistsAsync(authorizedUser.Id, request.OtherUserId);

        if (!doesMatchExists)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "You can't have conversation with user that you have not matched with.");
        }

        await _messageRepository.ReadConversation(readerId: authorizedUser.Id, otherUserId: request.OtherUserId);

        await _hubContext.Clients.Group(request.OtherUserId.ToString()).SendAsync("ConversationSeen", new
        {
            userId = request.OtherUserId,
            timestamp = DateTime.UtcNow
        }, cancellationToken);

        return Result.Success();
    }
}
