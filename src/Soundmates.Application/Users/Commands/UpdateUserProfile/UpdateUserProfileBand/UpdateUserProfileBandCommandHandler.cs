using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using static Soundmates.Domain.Constants.AppConstants;

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

        if (request.BandMembers.Count > MaxBandMembersCount)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Maximum number of band members is: {MaxBandMembersCount}");
        }

        authorizedUser.Name = request.Name;
        authorizedUser.Description = request.Description;
        authorizedUser.CountryId = request.CountryId;
        authorizedUser.CityId = request.CityId;

        var band = new Band { 
            UserId = authorizedUser.Id,
            User = authorizedUser
        };

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

        await _bandRepository.UpdateAddAsync(band, request.TagsIds, request.MusicSamplesOrder, request.ProfilePicturesOrder);

        return Result.Success();
    }
}
