using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [MaxLength(100)] 
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; } = null;

    [MaxLength(500)]
    public string? Description { get; set; } = null;

    public int? BirthYear { get; set; } = null;

    [MaxLength(100)]
    public string? City { get; set; } = null;

    [MaxLength(100)]
    public string? Country { get; set; } = null;

    public bool IsActive { get; set; } = true;

    public bool IsFirstLogin { get; set; } = true;

    public bool IsEmailConfirmed { get; set; } = true;

    public bool IsLoggedOut { get; set; } = false;
}
