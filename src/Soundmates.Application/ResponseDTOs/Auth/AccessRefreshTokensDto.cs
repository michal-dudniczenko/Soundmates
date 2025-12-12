namespace Soundmates.Application.ResponseDTOs.Auth;

public class AccessRefreshTokensDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
