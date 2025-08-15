namespace Soundmates.Api.DTOs.Users;

public class ChangePasswordDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
