namespace Soundmates.Api.DTOs.Auth;

public abstract class CredentialsDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
