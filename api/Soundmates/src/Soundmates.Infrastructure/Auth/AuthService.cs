using Soundmates.Domain.Interfaces.Auth;

namespace Soundmates.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly string _secretKey;

    public AuthService(string secretKey)
    {
        if (String.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key cannot be empty.", nameof(secretKey));
        }

        _secretKey = secretKey;
    }

    public string GenerateAccessToken(int userId)
    {
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public string GetPasswordHash(string password)
    {
        throw new NotImplementedException();
    }

    public string GetRefreshTokenHash(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public bool ValidatePasswordStrength(string password)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        throw new NotImplementedException();
    }

    public bool VerifyRefreshToken(string refreshToken, string refreshTokenHash)
    {
        throw new NotImplementedException();
    }
}
