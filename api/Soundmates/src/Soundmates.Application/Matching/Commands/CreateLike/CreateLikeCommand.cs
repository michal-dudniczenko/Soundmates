using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Commands.CreateLike;

public record CreateLikeCommand(string? SubClaim, int ReceiverId) : IRequest<Result>;
