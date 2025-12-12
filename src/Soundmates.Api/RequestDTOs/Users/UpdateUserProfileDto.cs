using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Api.RequestDTOs.Users;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(UpdateUserProfileArtistDto), "artist")]
[JsonDerivedType(typeof(UpdateUserProfileBandDto), "band")]
public abstract class UpdateUserProfileDto
{
    [Required]
    [MaxLength(MaxUserNameLength)]
    public required string Name { get; set; }

    [MaxLength(MaxUserDescriptionLength)]
    public required string Description { get; set; }

    [Required]
    public required Guid CountryId { get; set; }

    [Required]
    public required Guid CityId { get; set; }

    [Required]
    public required IList<Guid> TagsIds { get; set; }

    [Required]
    public required IList<Guid> MusicSamplesOrder { get; set; }

    [Required]
    public required IList<Guid> ProfilePicturesOrder { get; set; }
}
