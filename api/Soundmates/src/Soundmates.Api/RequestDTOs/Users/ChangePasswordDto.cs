using Soundmates.Api.RequestDTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class ChangePasswordDto
{
    [Required]
    public required string OldPassword { get; set; }

    [Required]
    [Password]
    public required string NewPassword { get; set; }
}
