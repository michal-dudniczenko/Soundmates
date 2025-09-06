using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Matching.Commands.CreateDislike;

public record CreateDislikeCommand(string? SubClaim, int ReceiverId) : IRequest<Result>;
