using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public class User
{
    public int Id { get; set; }

    [MaxLength(100)] 
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(50)]
    [RegularExpression(@"\S+", ErrorMessage = "Name cannot be empty or whitespace.")]
    public required string Name { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    [BirthYear]
    public int BirthYear { get; set; }

    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "City cannot be empty or whitespace.")]
    public required string City { get; set; }

    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "Country cannot be empty or whitespace.")]
    public required string Country { get; set; }

    public bool IsActive { get; set; } = true;

    public required string PasswordHash { get; set; }

    public string? RefreshTokenHash { get; set; } = null;
}
