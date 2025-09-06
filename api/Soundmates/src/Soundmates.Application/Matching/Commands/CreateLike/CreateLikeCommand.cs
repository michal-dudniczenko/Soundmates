using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Commands.CreateLike;

public record CreateLikeCommand(string? SubClaim, Guid ReceiverId) : IRequest<Result>;
