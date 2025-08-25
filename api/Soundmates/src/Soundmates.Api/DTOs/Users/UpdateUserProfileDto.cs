using Soundmates.Api.DTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Users;

public class UpdateUserProfileDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    [BirthYear]
    public required int BirthYear { get; set; }

    [Required]
    [MaxLength(100)]
    public required string City { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Country { get; set; }
}
