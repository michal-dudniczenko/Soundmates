using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Messages;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Messages.Queries.GetConversation;

public class GetConversationQueryHandler(
    IUserRepository _userRepository,
    IMessageRepository _messageRepository,
    IAuthService _authService
) : IRequestHandler<GetConversationQuery, Result<List<MessageDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<MessageDto>>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<MessageDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<MessageDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var otherUser = await _userRepository.GetByIdAsync(request.OtherUserId);

        if (otherUser is null || otherUser.IsFirstLogin)
        {
            return Result<List<MessageDto>>.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"User with id {request.OtherUserId} not found.");
        }

        var conversation = await _messageRepository.GetConversationAsync(
            user1Id: authorizedUser.Id,
            user2Id: request.OtherUserId,
            limit: request.Limit,
            offset: request.Offset);

        var conversationDtos = conversation.Select(m => new MessageDto
        {
            Content = m.Content,
            Timestamp = m.Timestamp,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            IsSeen = m.IsSeen
        }).ToList();

        return Result<List<MessageDto>>.Success(conversationDtos);
    }
}
