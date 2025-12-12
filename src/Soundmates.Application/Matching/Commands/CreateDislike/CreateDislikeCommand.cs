using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Commands.CreateDislike;

public record CreateDislikeCommand(string? SubClaim, Guid ReceiverId) : IRequest<Result>;
