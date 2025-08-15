namespace Soundmates.Api.DTOs.Auth;

public class AccessRefreshTokensDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
