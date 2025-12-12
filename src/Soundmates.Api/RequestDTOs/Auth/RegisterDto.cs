using Soundmates.Api.RequestDTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Api.RequestDTOs.Auth;

public class RegisterDto
{
    [Required]
    [MaxLength(MaxUserEmailLength)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [Password]
    public required string Password { get; set; }
}
