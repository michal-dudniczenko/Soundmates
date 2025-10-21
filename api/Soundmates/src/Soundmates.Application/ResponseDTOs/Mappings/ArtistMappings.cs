using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Entities;
using static Soundmates.Application.Common.UserMediaHelpers;

namespace Soundmates.Application.ResponseDTOs.Mappings;

public static class ArtistMappings
{
    public static OtherUserProfileArtistDto ToOtherUserProfileDto(this Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist);

        return new OtherUserProfileArtistDto
        {
            Id = artist.User.Id,
            IsBand = artist.User.IsBand,
            Name = artist.User.Name!,
            Description = artist.User.Description!,
            CountryId = artist.User.CountryId,
            CityId = artist.User.CityId,
            TagsIds = artist.User.Tags.Select(t => t.Id).ToList(),
            MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BirthDate = artist.BirthDate
        };
    }

    public static SelfUserProfileArtistDto ToSelfUserProfileDto(this Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist);

        return new SelfUserProfileArtistDto
        {
            Id = artist.User.Id,
            IsBand = artist.User.IsBand,
            Email = artist.User.Email,
            Name = artist.User.Name!,
            Description = artist.User.Description!,
            CountryId = artist.User.CountryId,
            CityId = artist.User.CityId,
            IsFirstLogin = artist.User.IsFirstLogin,
            TagsIds = artist.User.Tags.Select(t => t.Id).ToList(),
            MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BirthDate = artist.BirthDate
        };
    }
}
