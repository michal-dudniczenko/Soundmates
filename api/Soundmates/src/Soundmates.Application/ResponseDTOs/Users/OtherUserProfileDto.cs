using System.Text.Json.Serialization;

namespace Soundmates.Application.ResponseDTOs.Users;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(OtherUserProfileArtistDto), "artist")]
[JsonDerivedType(typeof(OtherUserProfileBandDto), "band")]
public abstract class OtherUserProfileDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Guid CountryId { get; set; }
    public required Guid CityId { get; set; }
    public required IList<Guid> TagIds { get; set; }
    public required IList<MusicSampleDto> MusicSamples { get; set; }
    public required IList<ProfilePictureDto> ProfilePictures { get; set; }
}
