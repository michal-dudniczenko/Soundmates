namespace Soundmates.Domain.Interfaces.Auth;

public interface IAuthService
{
    string GetPasswordHash(string password);
    string GetRefreshTokenHash(string refreshToken);
    string GenerateAccessToken(int userId);
    string GenerateRefreshToken(int userId);
    bool VerifyPasswordHash(string password, string passwordHash);
    bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash);
}
