using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Auth;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<RefreshCommand, Result<AccessTokenDto>>
{
    public async Task<Result<AccessTokenDto>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = _authService.GetRefreshTokenHash(request.RefreshToken);

        var userId = await _userRepository.CheckRefreshTokenGetUserIdAsync(refreshTokenHash: refreshTokenHash);
        if (userId is null)
        {
            return Result<AccessTokenDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid refresh token. Log in to get a new one.");
        }

        var accessToken = _authService.GenerateAccessToken(userId: (Guid)userId);

        var authAccessToken = new AccessTokenDto
        {
            AccessToken = accessToken
        };

        return Result<AccessTokenDto>.Success(authAccessToken);
    }
}
