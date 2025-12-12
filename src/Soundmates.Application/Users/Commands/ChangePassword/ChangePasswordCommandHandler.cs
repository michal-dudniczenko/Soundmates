using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (!_authService.VerifyPasswordHash(password: request.OldPassword, passwordHash: authorizedUser.PasswordHash))
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Old password is incorrect.");
        }

        var newPasswordHash = _authService.GetPasswordHash(request.NewPassword);

        await _userRepository.UpdateUserPasswordAsync(userId: authorizedUser.Id, newPasswordHash: newPasswordHash);

        return Result.Success();
    }
}
