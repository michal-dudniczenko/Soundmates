using Microsoft.AspNetCore.Identity;
using Soundmates.Domain.Interfaces.Auth;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Soundmates.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly PasswordHasher<object> _passwordHasher;

    public AuthService(string secretKey)
    {
        if (String.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key cannot be empty.", nameof(secretKey));
        }

        _secretKey = secretKey;
        _passwordHasher = new PasswordHasher<object>();
    }

    public string GenerateAccessToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(30);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public string GetPasswordHash(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public string GetRefreshTokenHash(string refreshToken)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hash);
    }

    public bool ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        // Reject if contains anything outside standard printable ASCII range (33–126)
        if (password.Any(ch => ch < 33 || ch > 126))
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(user: null!, hashedPassword: passwordHash, providedPassword: password);
        return  result != PasswordVerificationResult.Failed;
    }

    public bool VerifyRefreshToken(string refreshToken, string refreshTokenHash)
    {
        string computedHash = GetRefreshTokenHash(refreshToken);
        return computedHash == refreshTokenHash;
    }
}
