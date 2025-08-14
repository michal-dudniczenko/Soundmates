namespace Soundmates.Domain.Interfaces.Auth;

public interface IAuthService
{
    string GetPasswordHash(string password);
    string GetRefreshTokenHash(string refreshToken);
    bool ValidatePasswordStrength(string password);
    string GenerateAccessToken(int userId);
    string GenerateRefreshToken();
    bool VerifyPassword(string password, string passwordHash);
    bool VerifyRefreshToken(string refreshToken, string refreshTokenHash);
}
