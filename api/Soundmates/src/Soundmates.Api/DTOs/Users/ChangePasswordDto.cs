using Soundmates.Api.DTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Users;

public class ChangePasswordDto
{
    [Required]
    public required string OldPassword { get; set; }

    [Required]
    [Password]
    public required string NewPassword { get; set; }
}
