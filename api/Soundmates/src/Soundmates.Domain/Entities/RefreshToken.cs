namespace Soundmates.Domain.Entities;

public class RefreshToken
{
    public required Guid UserId { get; set; }
    public required string RefreshTokenHash { get; set; }
    public required DateTime RefreshTokenExpiresAt { get; set; }
}
