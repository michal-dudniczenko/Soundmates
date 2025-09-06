using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Commands.DeactivateAccount;

public class DeactivateAccountCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<DeactivateAccountCommand, Result>
{
    public async Task<Result> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (!_authService.VerifyPasswordHash(password: request.Password, passwordHash: authorizedUser.PasswordHash))
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid password.");

        }

        await _userRepository.DeactivateUserAccountAsync(authorizedUser.Id);

        return Result.Success();
    }
}
