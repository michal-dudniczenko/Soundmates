using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [MaxLength(100)] 
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public bool IsBand { get; set; } = false;

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
    public bool IsFirstLogin { get; set; } = true;
    public bool IsEmailConfirmed { get; set; } = true;
    public bool IsLoggedOut { get; set; } = false;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    public ICollection<Tag> Tags { get; } = [];
    public ICollection<ProfilePicture> ProfilePictures { get; } = [];
    public ICollection<MusicSample> MusicSamples { get; } = [];
    public ICollection<Artist> Artists { get; } = [];
    public ICollection<Band> Bands { get; } = [];
}
