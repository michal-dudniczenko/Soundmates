using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;

public class UpdateUserProfileArtistCommandHandler(
    IArtistRepository _artistRepository,
    IAuthService _authService
) : IRequestHandler<UpdateUserProfileArtistCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileArtistCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        authorizedUser.IsBand = false;

        authorizedUser.Name = request.Name;
        authorizedUser.Description = request.Description;
        authorizedUser.IsFirstLogin = false;
        authorizedUser.CountryId = request.CountryId;
        authorizedUser.CityId = request.CityId;

        authorizedUser.Tags.Clear();
        foreach (var tag in request.Tags)
        {
            authorizedUser.Tags.Add(new Tag
            {
                Id = tag.Id,
                Name = tag.Name,
                TagCategoryId = tag.TagCategoryId
            });
        }

        var artist = await _artistRepository.GetByUserIdAsync(authorizedUser.Id) ?? new Artist { UserId = authorizedUser.Id };

        artist.BirthDate = request.BirthDate;
        artist.GenderId = request.GenderId;

        artist.User = authorizedUser;

        await _artistRepository.UpdateAddAsync(artist, request.MusicSamplesOrder, request.ProfilePicturesOrder);

        return Result.Success();
    }
}
