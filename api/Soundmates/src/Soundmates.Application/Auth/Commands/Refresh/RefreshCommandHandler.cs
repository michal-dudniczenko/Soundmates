using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<RefreshCommand, Result<AuthAccessToken>>
{
    public async Task<Result<AuthAccessToken>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = _authService.GetRefreshTokenHash(request.RefreshToken);

        var userId = await _userRepository.CheckRefreshTokenGetUserIdAsync(refreshTokenHash: refreshTokenHash);
        if (userId is null)
        {
            return Result<AuthAccessToken>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid refresh token. Log in to get a new one.");
        }

        var accessToken = _authService.GenerateAccessToken(userId: (int)userId);

        var authAccessToken = new AuthAccessToken
        {
            AccessToken = accessToken
        };

        return Result<AuthAccessToken>.Success(authAccessToken);
    }
}
