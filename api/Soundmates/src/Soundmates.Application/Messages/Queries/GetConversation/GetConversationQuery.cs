using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Messages.Common;

namespace Soundmates.Application.Messages.Queries.GetConversation;

public record GetConversationQuery(int OtherUserId, int Limit, int Offset, string? SubClaim) : IRequest<Result<List<MessageDto>>>;