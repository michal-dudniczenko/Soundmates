using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Messages;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Messages.Queries.GetConversationsPreview;

public class GetConversationsPreviewQueryHandler(
    IMessageRepository _messageRepository,
    IAuthService _authService
) : IRequestHandler<GetConversationsPreviewQuery, Result<List<MessageDto>>>
{
    public async Task<Result<List<MessageDto>>> Handle(GetConversationsPreviewQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<MessageDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var lastMessages = await _messageRepository.GetConversationsPreviewAsync(userId: authorizedUser.Id);

        var lastMessagesDtos = lastMessages.Select(m => new MessageDto
        {
            Content = m.Content,
            Timestamp = m.Timestamp,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            IsSeen = m.IsSeen
        }).ToList();

        return Result<List<MessageDto>>.Success(lastMessagesDtos);
    }
}
