using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Users;

public class PasswordDto
{
    [Required]
    public required string Password { get; set; }
}
