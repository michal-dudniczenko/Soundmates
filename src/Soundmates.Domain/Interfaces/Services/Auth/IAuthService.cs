using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Services.Auth;

public interface IAuthService
{
    string GetPasswordHash(string password);
    string GetRefreshTokenHash(string refreshToken);
    string GenerateAccessToken(Guid userId);
    string GenerateRefreshToken(Guid userId);
    bool VerifyPasswordHash(string password, string passwordHash);
    bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash);
    Task<User?> GetAuthorizedUserAsync(string? subClaim, bool checkForFirstLogin);
}
