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

        if (authorizedUser.IsLoggedOut)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "User is already logged out.");
        }

        await _userRepository.LogOutUserAsync(userId: authorizedUser.Id);

        return Result.Success();
    }
}
