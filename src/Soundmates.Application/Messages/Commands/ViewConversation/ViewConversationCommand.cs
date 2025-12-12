using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Messages.Commands.ViewConversation;

public record ViewConversationCommand(Guid OtherUserId, string? SubClaim) : IRequest<Result>;