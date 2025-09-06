using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Soundmates.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly PasswordHasher<object> _passwordHasher;

    private readonly IUserRepository _userRepository;

    public AuthService(string secretKey, IUserRepository userRepository)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key cannot be empty.", nameof(secretKey));
        }

        _secretKey = secretKey;
        _passwordHasher = new PasswordHasher<object>();
        _userRepository = userRepository;
    }

    public string GenerateAccessToken(Guid userId)
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

    public string GenerateRefreshToken(Guid userId)
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

    public bool VerifyPasswordHash(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(user: null!, hashedPassword: passwordHash, providedPassword: password);
        return  result != PasswordVerificationResult.Failed;
    }

    public bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash)
    {
        string computedHash = GetRefreshTokenHash(refreshToken);
        return computedHash == refreshTokenHash;
    }

    public async Task<User?> GetAuthorizedUserAsync(string? subClaim, bool checkForFirstLogin)
    {
        if (!Guid.TryParse(subClaim, out var authorizedUserId))
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(authorizedUserId);
        if (user is null || user.IsLoggedOut || (checkForFirstLogin && user.IsFirstLogin))
        {
            return null;
        }

        return user;
    }
}
