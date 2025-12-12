using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword, string? SubClaim) : IRequest<Result>;
