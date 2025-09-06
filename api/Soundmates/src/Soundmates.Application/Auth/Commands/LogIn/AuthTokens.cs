namespace Soundmates.Application.Auth.Commands.LogIn;

public class AuthTokens
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
