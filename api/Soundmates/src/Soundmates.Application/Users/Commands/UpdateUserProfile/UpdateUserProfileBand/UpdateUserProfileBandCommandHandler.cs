using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;

public class UpdateUserProfileBandCommandHandler(
    IBandRepository _bandRepository,
    IAuthService _authService
) : IRequestHandler<UpdateUserProfileBandCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileBandCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        authorizedUser.IsBand = true;

        authorizedUser.Name = request.Name;
        authorizedUser.Description = request.Description;
        authorizedUser.IsFirstLogin = false;
        authorizedUser.CountryId = request.CountryId;
        authorizedUser.CityId = request.CityId;

        var band = await _bandRepository.GetByUserIdAsync(authorizedUser.Id) ?? new Band { UserId = authorizedUser.Id };

        band.Members.Clear();
        int displayOrder = 0;
        foreach (var member in request.BandMembers)
        {
            band.Members.Add(new BandMember
            {
                Name = member.Name,
                Age = member.Age,
                DisplayOrder = displayOrder,
                BandId = band.Id,
                BandRoleId = member.BandRoleId
            });

            displayOrder++;
        }

        band.User = authorizedUser;

        await _bandRepository.UpdateAddAsync(band, request.TagsIds, request.MusicSamplesOrder, request.ProfilePicturesOrder);

        return Result.Success();
    }
}
