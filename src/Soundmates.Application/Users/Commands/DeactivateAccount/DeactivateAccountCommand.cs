using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.DeactivateAccount;

public record DeactivateAccountCommand(string Password, string? SubClaim) : IRequest<Result>;
