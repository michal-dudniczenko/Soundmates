using Soundmates.Api.RequestDTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Auth;

public class RegisterDto
{
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [Password]
    public required string Password { get; set; }
}
