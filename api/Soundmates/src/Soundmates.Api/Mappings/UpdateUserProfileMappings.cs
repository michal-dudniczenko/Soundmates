using Soundmates.Api.RequestDTOs.Users;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Application.Users.Commands.UpdateUserProfile;
using Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;
using Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;

namespace Soundmates.Api.Mappings;

public static class UpdateUserProfileMappings
{
    public static UpdateUserProfileCommand ToCommand(this UpdateUserProfileDto updateUserDto, string? subClaim)
    {
        return updateUserDto switch
        {
            UpdateUserProfileArtistDto dto => new UpdateUserProfileArtistCommand(
                Name: dto.Name.Trim(),
                Description: dto.Description.Trim(),
                CountryId: dto.CountryId,
                CityId: dto.CityId,
                TagsIds: dto.TagsIds,
                MusicSamplesOrder: dto.MusicSamplesOrder,
                ProfilePicturesOrder: dto.ProfilePicturesOrder,
                BirthDate: dto.BirthDate,
                GenderId: dto.GenderId,
                SubClaim: subClaim),
            UpdateUserProfileBandDto dto => new UpdateUserProfileBandCommand(
                Name: dto.Name.Trim(),
                Description: dto.Description.Trim(),
                CountryId: dto.CountryId,
                CityId: dto.CityId,
                TagsIds: dto.TagsIds,
                MusicSamplesOrder: dto.MusicSamplesOrder,
                ProfilePicturesOrder: dto.ProfilePicturesOrder,
                BandMembers: dto.BandMembers.Select(bm => new BandMemberDto
                {
                    Id = bm.Id,
                    Name = bm.Name.Trim(),
                    Age = bm.Age,
                    BandRoleId = bm.BandRoleId
                }).ToList(),
                SubClaim: subClaim),
            _ => throw new InvalidOperationException()
        };
    }
}
