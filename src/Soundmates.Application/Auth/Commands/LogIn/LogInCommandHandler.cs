using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Auth;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.LogIn;

public class LogInCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
): IRequestHandler<LogInCommand, Result<AccessRefreshTokensDto>>
{
    public async Task<Result<AccessRefreshTokensDto>> Handle(LogInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email: request.Email);

        if (user is null)
        {
            return Result<AccessRefreshTokensDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid email or password.");
        }

        var verifyHashResult = _authService.VerifyPasswordHash(password: request.Password, passwordHash: user.PasswordHash);

        if (!verifyHashResult)
        {
            return Result<AccessRefreshTokensDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return Result<AccessRefreshTokensDto>.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "Your account has been deactivated. Contact administrator.");
        }

        var accessToken = _authService.GenerateAccessToken(userId: user.Id);
        var refreshToken = _authService.GenerateRefreshToken(userId: user.Id);
        var refreshTokenHash = _authService.GetRefreshTokenHash(refreshToken);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);

        await _userRepository.LogInUserAsync(userId: user.Id, newRefreshTokenHash: refreshTokenHash, newRefreshTokenExpiresAt: refreshTokenExpiresAt);

        var authTokens = new AccessRefreshTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result<AccessRefreshTokensDto>.Success(authTokens);
    }
}
