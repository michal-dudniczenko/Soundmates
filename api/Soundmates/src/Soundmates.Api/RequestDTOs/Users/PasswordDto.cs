using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class PasswordDto
{
    [Required]
    public required string Password { get; set; }
}
