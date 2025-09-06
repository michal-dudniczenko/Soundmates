using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Messages.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Messages.Queries.GetConversationsPreview;

public class GetConversationsPreviewQueryHandler(
    IMessageRepository _messageRepository,
    IAuthService _authService
) : IRequestHandler<GetConversationsPreviewQuery, Result<List<MessageDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<MessageDto>>> Handle(GetConversationsPreviewQuery request, CancellationToken cancellationToken)
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

        var lastMessages = await _messageRepository.GetConversationsLastMessagesAsync(
                userId: authorizedUser.Id,
                limit: request.Limit,
                offset: request.Offset);

        var lastMessagesDtos = lastMessages.Select(m => new MessageDto
        {
            Content = m.Content,
            Timestamp = m.Timestamp,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId
        }).ToList();

        return Result<List<MessageDto>>.Success(lastMessagesDtos);
    }
}
