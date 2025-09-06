using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Messages.Commands.SendMessage;

public class SendMessageCommandHandler(
    IUserRepository _userRepository,
    IMessageRepository _messageRepository,
    IMatchRepository _matchRepository,
    IAuthService _authService
) : IRequestHandler<SendMessageCommand, Result>
{
    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var otherUserExists = await _userRepository.CheckIfExistsActiveAsync(request.ReceiverId);

        if (!otherUserExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.ReceiverId}");
        }

        var doesMatchExists = await _matchRepository.CheckIfMatchExistsAsync(authorizedUser.Id, request.ReceiverId);

        if (!doesMatchExists)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "You can send message only to users you have matched with.");
        }

        var message = new Message
        {
            Content = request.Content,
            SenderId = authorizedUser.Id,
            ReceiverId = request.ReceiverId
        };

        await _messageRepository.AddAsync(message);

        return Result.Success();
    }
}
