using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Messages;

namespace Soundmates.Application.Messages.Queries.GetConversation;

public record GetConversationQuery(Guid OtherUserId, int Limit, int Offset, string? SubClaim) : IRequest<Result<List<MessageDto>>>;