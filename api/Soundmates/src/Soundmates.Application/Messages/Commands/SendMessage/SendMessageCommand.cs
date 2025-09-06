using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Messages.Commands.SendMessage;

public record SendMessageCommand(int ReceiverId, string Content, string? SubClaim) : IRequest<Result>;