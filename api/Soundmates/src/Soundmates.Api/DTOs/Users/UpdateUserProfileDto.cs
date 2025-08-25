using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Users;

public class UpdateUserProfileDto
{
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"\S+", ErrorMessage = "Name cannot be empty or whitespace.")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Description { get; set; }

    [BirthYear]
    public required int BirthYear { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "City cannot be empty or whitespace.")]
    public required string City { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "Country cannot be empty or whitespace.")]
    public required string Country { get; set; }
}
