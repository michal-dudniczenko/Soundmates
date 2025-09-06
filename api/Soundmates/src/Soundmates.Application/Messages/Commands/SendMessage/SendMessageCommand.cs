using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Messages.Commands.SendMessage;

public record SendMessageCommand(Guid ReceiverId, string Content, string? SubClaim) : IRequest<Result>;