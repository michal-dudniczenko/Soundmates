using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Auth;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
