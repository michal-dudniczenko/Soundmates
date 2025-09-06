using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.LogOut;

public class LogOutCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<LogOutCommand, Result>
{
    public async Task<Result> Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var logOutResult = await _userRepository.LogOutUserAsync(userId: authorizedUser.Id);

        if (!logOutResult)
        {
            return Result.Failure(
                errorType: ErrorType.InternalServerError,
                errorMessage: "Something went wrong. Failed to log out.");
        }

        return Result.Success();
    }
}
