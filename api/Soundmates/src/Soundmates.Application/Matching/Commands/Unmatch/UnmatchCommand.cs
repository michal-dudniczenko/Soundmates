using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Commands.Unmatch;

public record UnmatchCommand(Guid OtherUserId, string? SubClaim) : IRequest<Result>;
