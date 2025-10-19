namespace Soundmates.Domain.Entities;

public class UserMatchPreference
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public bool ShowArtists { get; set; } = true;
    public bool ShowBands { get; set; } = true;

    public int? MaxDistance { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? CityId { get; set; }

    public int? ArtistMinAge { get; set; }
    public int? ArtistMaxAge { get; set; }
    public Guid? ArtistGenderId { get; set; }

    public int? BandMinMembersCount { get; set; }
    public int? BandMaxMembersCount { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<UserMatchTagPreference> UserMatchTagPreferences { get; } = [];
}
